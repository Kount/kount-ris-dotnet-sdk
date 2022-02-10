using KountRisSdk.Kount.Log.Factory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kount.Ris
{
    /// <summary>
    /// Logging Container
    /// </summary>
    public abstract class LoggingComponent
    {
        private static ILogger _defaultLogger = NullLogger.Instance;

        /// <summary>
        /// Default Logger
        /// </summary>
        public static ILogger defaultLogger
        {
            get => _defaultLogger != null ? _defaultLogger : (_defaultLogger = NullLogger.Instance);
            set => _defaultLogger = value;
        }

        private ILogger _logger = null;

        /// <summary>
        /// Logger use
        /// </summary>
        public ILogger logger
        {
            get => _logger != null ? _logger : (_logger = defaultLogger);
            set => _logger = value;
        }

        KountRisSdk.Kount.Log.Factory.ILoggerFactory factory = LogFactory.GetLoggerFactory();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="type"></param>
        public LoggingComponent(ILogger logger = null, Type type = null)
        {
            if (logger == null)
            {
                logger = factory.GetLogger(type.FullName);
            }

            this.logger = logger;
        }
    }
}