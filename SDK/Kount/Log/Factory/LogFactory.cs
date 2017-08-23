//-----------------------------------------------------------------------
// <copyright file="LogFactory.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log.Factory
{
    using System;
    using System.Configuration;

    /// <summary>
    /// A factory class for creating LoggerFactory objects.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class LogFactory
    {
        /// <summary>
        /// NOP logger string
        /// </summary>
        private const string NopLogger = "NOP";

        /// <summary>
        /// Simple logger string
        /// </summary>
        private const string SimpleLogger = "SIMPLE";

        /// <summary>
        /// The logger factory
        /// </summary>
        private static ILoggerFactory factory = null;

        /// <summary>
        /// Get the logger factory to be used.
        /// <b>NOP</b> is default logger factory if not defined in application configuration.
        /// </summary>
        /// <returns>A Kount.Log.Factory.LoggerFactory</returns>
        public static ILoggerFactory GetLoggerFactory()
        {
            if (factory == null)
            {
                string logger = ConfigurationManager.AppSettings["LOG.LOGGER"];

                if (logger == null)
                {
                    return new NopLoggerFactory();
                }

                if (logger.Equals(NopLogger))
                {
                    factory = new NopLoggerFactory();
                }
                else if (logger.Equals(SimpleLogger))
                {
                    factory = new SimpleLoggerFactory();
                }
                else
                {
                    throw new Exception($"Unknown logger [{logger}] defined in application configuration.");
                }
            }

            return factory;
        }

        /// <summary>
        /// Set the logger factory to be used.
        /// </summary>
        /// <param name="f">A Kount.Log.Factory.LoggerFactory</param>
        public static void SetLoggerFactory(ILoggerFactory f)
        {
            factory = f;
        }
    }
}