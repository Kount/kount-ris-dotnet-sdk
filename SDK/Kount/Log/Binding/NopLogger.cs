//-----------------------------------------------------------------------
// <copyright file="NopLogger.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log.Binding
{
    using System;

    /// <summary>
    /// A logger that silently discards all logging.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class NopLogger : ILogger
    {
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

        /// <summary>
        /// Discard a debug level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
        }

        /// <summary>
        /// Discard a debug level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Debug(string message, Exception e)
        {
        }

        /// <summary>
        /// Discard an info level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Info(string message)
        {
        }

        /// <summary>
        /// Discard an info level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Info(string message, Exception e)
        {
        }

        /// <summary>
        /// Discard a warn level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warn(string message)
        {
        }

        /// <summary>
        /// Discard a warn level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Warn(string message, Exception e)
        {
        }

        /// <summary>
        /// Discard an error level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
        }

        /// <summary>
        /// Discard an error level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Error(string message, Exception e)
        {
        }

        /// <summary>
        /// Discard a fatal level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Fatal(string message)
        {
        }

        /// <summary>
        /// Discard a fatal level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        public void Fatal(string message, Exception e)
        {
        }
    }
}