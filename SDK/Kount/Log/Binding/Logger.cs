//-----------------------------------------------------------------------
// <copyright file="Logger.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log.Binding
{
    using System;

    /// <summary>
    /// Logger interface.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a debug level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        void Debug(string message);

        /// <summary>
        /// Log a debug level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        void Debug(string message, Exception e);

        /// <summary>
        /// Log an info level message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Info(string message);

        /// <summary>
        /// Log an info level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        void Info(string message, Exception e);

        /// <summary>
        /// Log a warn level message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Warn(string message);

        /// <summary>
        /// Log a warn level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        void Warn(string message, Exception e);

        /// <summary>
        /// Log an error level message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Error(string message);

        /// <summary>
        /// Log an error level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        void Error(string message, Exception e);

        /// <summary>
        /// Log a fatal level message
        /// </summary>
        /// <param name="message">Message to log</param>
        void Fatal(string message);

        /// <summary>
        /// Log a fatal level message and an exception.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="e">Exception to log</param>
        void Fatal(string message, Exception e);

        /// <summary>
        /// Configurable flag. In `app.config` set setting `LOG.SIMPLE.ELAPSED` to <b>ON/OFF</b><br/>
        /// example: 
        /// <example>`<add key="LOG.SIMPLE.ELAPSED" value="ON" />`</example><br/>
        /// When is `true` - measure overall client request time in milliseconds and logging result.<br/>
        /// By default is `false`(OFF)
        /// </summary>
        bool MeasureElapsed { get; }
    }
}