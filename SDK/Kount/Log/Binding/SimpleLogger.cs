//-----------------------------------------------------------------------
// <copyright file="SimpleLogger.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log.Binding
{
    using Kount.SimpleLogger;
    using System;

    /// <summary>
    /// Facade class to a simple file logger.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class SimpleLogger : ILogger
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
        /// Log a debug level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
            this.logger.Debug(message);
        }

        /// <summary>
        /// Log a debug level message and exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Debug(string message, Exception e)
        {
            this.logger.Debug(message, e);
        }

        /// <summary>
        /// Log an info level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Info(string message)
        {
            this.logger.Info(message);
        }

        /// <summary>
        /// Log an info level message and exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Info(string message, Exception e)
        {
            this.logger.Info(message, e);
        }

        /// <summary>
        /// Log a warn level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warn(string message)
        {
            this.logger.Warn(message);
        }

        /// <summary>
        /// Log a warn level message and exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Warn(string message, Exception e)
        {
            this.logger.Warn(message, e);
        }

        /// <summary>
        /// Log an error level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
            this.logger.Error(message);
        }

        /// <summary>
        /// Log an error level message and exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Error(string message, Exception e)
        {
            this.logger.Error(message, e);
        }

        /// <summary>
        /// Log a fatal level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }

        /// <summary>
        /// Log a fatal level message and exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Fatal(string message, Exception e)
        {
            this.logger.Fatal(message, e);
        }
    }
}