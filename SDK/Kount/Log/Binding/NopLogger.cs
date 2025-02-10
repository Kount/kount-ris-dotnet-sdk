using System;
using System.Collections.Generic;
using System.Text;

namespace KountRisSdk.Kount.Log.Binding
{
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// A logger that silently discards all logging.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public class NopLogger : ILoggerSdk
    {
        /// <summary>
        /// Get whether to measure elapsed time
        /// </summary>
        public bool MeasureElapsed
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Constructor for NOP logger.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        public NopLogger(string name)
        {
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
