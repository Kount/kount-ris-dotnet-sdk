namespace PaymentsFraudTest
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Kount.Ris;
    using Xunit;

    /// <summary>
    /// Tests for GitHub Issue #24: RefreshAuthToken must release lock on exception
    /// https://github.com/Kount/kount-ris-dotnet-sdk/issues/24
    ///
    /// The bug: RefreshAuthToken() acquires a writer lock but doesn't use try/finally,
    /// so if an HTTP exception occurs, the static lock is NEVER released. This causes
    /// all subsequent SDK calls to timeout waiting for the lock (30-second timeout).
    ///
    /// The fix: Wrap the method body in try/finally to ensure ReleaseWriterLock() is
    /// always called, even when exceptions occur.
    /// </summary>
    [Collection("RefreshAuthTokenTests")] // Prevents parallel execution - these tests share static state
    public class RefreshAuthTokenLockReleaseTest : IDisposable
    {
        // SDK uses 30-second lock timeout. If we're waiting more than 2 seconds for a call
        // that should fail immediately, something is wrong with lock release.
        private const int QuickFailureThresholdMs = 2000;

        // Maximum acceptable time for multiple sequential operations
        // Should be << 30 seconds (the lock timeout) to prove lock is being released
        private const int MaxSequentialOperationsMs = 10000;

        // SDK's internal lock timeout
        private const int SdkLockTimeoutSeconds = 30;

        public RefreshAuthTokenLockReleaseTest()
        {
            // Ensure clean state before each test
            ResetStaticAuthState();
            EnsureLockIsReleased();
        }

        public void Dispose()
        {
            // Clean up after each test
            ResetStaticAuthState();
            EnsureLockIsReleased();
        }

        /// <summary>
        /// CORE TEST: Verifies that when RefreshAuthToken throws an exception,
        /// the writer lock is properly released so subsequent calls don't timeout.
        ///
        /// This directly tests the try/finally fix for GitHub Issue #24.
        ///
        /// Expected behavior WITH the fix: Both calls fail quickly (connection refused)
        /// Behavior WITHOUT the fix: Second call hangs for 30 seconds waiting for lock
        /// </summary>
        [Fact]
        public void RefreshAuthToken_WhenConnectionRefused_ReleasesLockForNextCall()
        {
            // Arrange: Use localhost with a port that's definitely not listening
            // This gives us instant connection refused - no DNS delays
            var config = CreateConfigWithUnreachableLocalhost();

            // Verify the lock is free before we start
            AssertLockIsNotHeld("Lock was held before test started - test isolation failure");

            // Act: First call - should fail with connection error but MUST release the lock
            var stopwatch = Stopwatch.StartNew();
            var firstException = Record.Exception(() =>
            {
                new Inquiry(true, config, null);
            });
            var firstCallTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            // Assert: First call failed (expected - invalid URL)
            Assert.NotNull(firstException);

            // Key assertion: Lock should be released after exception
            AssertLockIsNotHeld("Lock was still held after first call exception - try/finally fix not working");

            // Act: Second call - should also fail quickly, NOT wait 30 seconds for stuck lock
            var secondException = Record.Exception(() =>
            {
                new Inquiry(true, config, null);
            });
            stopwatch.Stop();
            var secondCallTime = stopwatch.ElapsedMilliseconds;

            // Assert: Second call also failed (expected)
            Assert.NotNull(secondException);

            // Assert: Second call completed quickly (proves lock was released)
            Assert.True(
                secondCallTime < QuickFailureThresholdMs,
                $"Second call took {secondCallTime}ms. If this is close to {SdkLockTimeoutSeconds}s, " +
                $"the lock was not released after the first exception. Expected <{QuickFailureThresholdMs}ms.");

            // Both calls should have similar timing (both hitting connection refused immediately)
            var timeDifference = Math.Abs(firstCallTime - secondCallTime);
            Assert.True(
                timeDifference < 1000,
                $"First call: {firstCallTime}ms, Second call: {secondCallTime}ms. " +
                $"Large difference suggests lock contention.");
        }

        /// <summary>
        /// Verifies multiple sequential calls all complete quickly.
        /// If the lock wasn't being released, each call would add 30 seconds.
        /// 5 calls * 30 seconds = 150 seconds WITHOUT the fix
        /// 5 calls * ~100ms = ~500ms WITH the fix
        /// </summary>
        [Fact]
        public void RefreshAuthToken_MultipleSequentialFailures_AllCompleteQuickly()
        {
            // Arrange
            var config = CreateConfigWithUnreachableLocalhost();
            const int callCount = 5;

            // Act
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < callCount; i++)
            {
                ResetStaticAuthState(); // Force refresh on each call

                var exception = Record.Exception(() => new Inquiry(true, config, null));
                Assert.NotNull(exception);
            }
            stopwatch.Stop();

            // Assert: Total time should be << 30 seconds
            // Without the fix: 5 * 30s = 150s (first succeeds in getting lock, rest timeout)
            // With the fix: 5 * ~100ms = ~500ms
            Assert.True(
                stopwatch.ElapsedMilliseconds < MaxSequentialOperationsMs,
                $"{callCount} sequential calls took {stopwatch.ElapsedMilliseconds}ms. " +
                $"Expected <{MaxSequentialOperationsMs}ms. If close to {callCount * SdkLockTimeoutSeconds}s, " +
                $"lock is not being released between calls.");
        }

        /// <summary>
        /// Directly verifies the ReaderWriterLock state using reflection.
        /// This is the most definitive test that the lock is released.
        /// </summary>
        [Fact]
        public void RefreshAuthToken_AfterException_CanImmediatelyAcquireLock()
        {
            // Arrange
            var config = CreateConfigWithUnreachableLocalhost();
            var lockField = GetLockField();
            var lockValue = lockField.GetValue(null);
            Assert.NotNull(lockValue);
            var readerWriterLock = (ReaderWriterLock)lockValue;

            // Act: Trigger a failed refresh
            var exception = Record.Exception(() => new Inquiry(true, config, null));
            Assert.NotNull(exception);

            // Assert: We should be able to acquire the lock immediately
            var stopwatch = Stopwatch.StartNew();
            bool acquiredLock = false;
            try
            {
                // Try to acquire with a very short timeout
                readerWriterLock.AcquireWriterLock(TimeSpan.FromMilliseconds(100));
                acquiredLock = true;
            }
            catch (ApplicationException)
            {
                // Lock acquisition timed out - this means lock is stuck
                acquiredLock = false;
            }
            finally
            {
                stopwatch.Stop();
                if (acquiredLock)
                {
                    readerWriterLock.ReleaseWriterLock();
                }
            }

            Assert.True(
                acquiredLock,
                $"Could not acquire writer lock after {stopwatch.ElapsedMilliseconds}ms. " +
                "The lock is stuck - try/finally fix not applied correctly.");

            Assert.True(
                stopwatch.ElapsedMilliseconds < 200,
                $"Lock acquisition took {stopwatch.ElapsedMilliseconds}ms, expected instant.");
        }

        /// <summary>
        /// Tests concurrent access to verify no permanent lock contention.
        /// Multiple threads should all fail quickly, not queue up behind a stuck lock.
        /// </summary>
        [Fact]
        public async Task RefreshAuthToken_ConcurrentCalls_AllResolveWithinReasonableTime()
        {
            // Arrange
            var config = CreateConfigWithUnreachableLocalhost();
            const int threadCount = 3;
            var exceptions = new Exception?[] { null, null, null };
            var completionTimes = new long[threadCount];

            // Act: Launch concurrent requests
            var tasks = new Task[threadCount];
            var startSignal = new ManualResetEventSlim(false);

            for (int i = 0; i < threadCount; i++)
            {
                int index = i;
                tasks[i] = Task.Run(() =>
                {
                    startSignal.Wait(); // All threads start simultaneously
                    var sw = Stopwatch.StartNew();
                    try
                    {
                        ResetStaticAuthState();
                        new Inquiry(true, config, null);
                    }
                    catch (Exception ex)
                    {
                        exceptions[index] = ex;
                    }
                    sw.Stop();
                    completionTimes[index] = sw.ElapsedMilliseconds;
                });
            }

            startSignal.Set(); // Release all threads

            // Wait with timeout using async await
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
            try
            {
                await Task.WhenAll(tasks).WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Assert.Fail("Some threads did not complete within 90 seconds - possible deadlock");
            }

            // Assert: All threads completed, all got exceptions
            for (int i = 0; i < threadCount; i++)
            {
                Assert.NotNull(exceptions[i]);
            }

            // Total time should be reasonable
            // Worst case with proper lock release: threads serialize, each takes ~100ms
            // Without fix: 30s * threadCount minimum
            var maxTime = completionTimes.Max();
            Assert.True(
                maxTime < SdkLockTimeoutSeconds * 1000,
                $"Slowest thread took {maxTime}ms. If this is ~{SdkLockTimeoutSeconds}s, " +
                "threads are waiting for stuck lock.");
        }

        #region Helper Methods

        /// <summary>
        /// Creates a Configuration pointing to localhost on a port that's not listening.
        /// This ensures connection refused errors happen instantly (no DNS lookup delay).
        /// </summary>
        private Configuration CreateConfigWithUnreachableLocalhost()
        {
            // Port 1 is reserved and won't have anything listening
            // Using 127.0.0.1 avoids any DNS resolution
            return new Configuration
            {
                MerchantId = "999999",
                URL = "https://127.0.0.1:1/api/v1/ris",
                ConfigKey = "0123456789ABCDEF0123456789ABCDEF", // 32 chars for Base85
                ConnectTimeout = "1000", // 1 second - fail fast
                EnableMigrationMode = "true",
                PaymentsFraudClientId = "123456", // Must be numeric - SDK parses as Int64
                // Auth URL pointing to non-listening localhost port - instant connection refused
                PaymentsFraudAuthUrl = "https://127.0.0.1:1/oauth2/token"
            };
        }

        /// <summary>
        /// Resets the static auth state to force a token refresh on next call.
        /// </summary>
        private void ResetStaticAuthState()
        {
            var expirationField = typeof(Request).GetField(
                "_bearerAuthResponseExpiration",
                BindingFlags.NonPublic | BindingFlags.Static);

            expirationField?.SetValue(null, DateTimeOffset.MinValue);

            var responseField = typeof(Request).GetField(
                "_bearerAuthResponse",
                BindingFlags.NonPublic | BindingFlags.Static);

            if (responseField != null)
            {
                var emptyResponse = Activator.CreateInstance(responseField.FieldType);
                responseField.SetValue(null, emptyResponse);
            }
        }

        /// <summary>
        /// Gets the private static ReaderWriterLock field via reflection.
        /// </summary>
        private FieldInfo GetLockField()
        {
            var field = typeof(Request).GetField(
                "_bearerRefreshLock",
                BindingFlags.NonPublic | BindingFlags.Static);

            Assert.NotNull(field);
            return field;
        }

        /// <summary>
        /// Asserts that the writer lock is not currently held.
        /// </summary>
        private void AssertLockIsNotHeld(string message)
        {
            var lockField = GetLockField();
            var lockValue = lockField.GetValue(null);
            if (lockValue == null)
            {
                Assert.Fail("Lock field is null - test cannot proceed");
                return;
            }
            var readerWriterLock = (ReaderWriterLock)lockValue;

            Assert.False(readerWriterLock.IsWriterLockHeld, message);
        }

        /// <summary>
        /// Ensures the lock is released. Called during test setup/teardown.
        /// </summary>
        private void EnsureLockIsReleased()
        {
            var lockField = GetLockField();
            var lockValue = lockField.GetValue(null);
            if (lockValue == null)
            {
                return; // Nothing to release
            }
            var readerWriterLock = (ReaderWriterLock)lockValue;

            // If the current thread somehow holds the lock, release it
            if (readerWriterLock.IsWriterLockHeld)
            {
                try
                {
                    readerWriterLock.ReleaseWriterLock();
                }
                catch
                {
                    // Ignore - just best effort cleanup
                }
            }
        }

        #endregion
    }
}
