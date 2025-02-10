namespace KountRisSdk.Kount.Log.Binding
{
    using Kount.SimpleLogger;
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Facade class to a simple file logger.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public class SimpleLogger : ILoggerSdk
    {
        /// <summary>
        /// File handle to use for logging
        /// </summary>
        private File logger;

        /// <summary>
        /// Configurable property. In `app.config` set setting `LOG.SIMPLE.ELAPSED` to <b>ON/OFF</b><br/>
        /// example: 
        /// <example>`<add key="LOG.SIMPLE.ELAPSED" value="ON" />`</example><br/>
        /// When is `true` - measure overall client request time in milliseconds and logging result.<br/>
        /// By default is `false`(OFF)
        /// </summary>
        public bool MeasureElapsed { get; }

        /// <summary>
        /// The Constructor.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        public SimpleLogger(string name)
        {
            this.logger = new File(name);
            this.MeasureElapsed = (String.IsNullOrEmpty(this.logger.SdkElapsed))
                                ? false
                                : this.logger.SdkElapsed.Trim().ToLower().Equals("on");
        }

        /// <summary>
        /// Log a message based on given logLevel
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    this.logger.Debug(state.ToString(), exception);
                    break;
                case LogLevel.Information:
                    this.logger.Info(state.ToString(), exception);
                    break;
                case LogLevel.Warning:
                    this.logger.Warn(state.ToString(), exception);
                    break;
                case LogLevel.Error:
                    this.logger.Error(state.ToString(), exception);
                    break;
                case LogLevel.Critical:
                    this.logger.Fatal(state.ToString(), exception);
                    break;
                default:
                    this.logger.Debug(state.ToString(), exception);
                    break;
            }
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
