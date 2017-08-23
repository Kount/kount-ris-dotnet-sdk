//-----------------------------------------------------------------------
// <copyright file="File.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.SimpleLogger
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A simple file logger.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class File
    {
        /// <summary>
        /// Debug log level
        /// </summary>
        private const string LogDebug = "DEBUG";

        /// <summary>
        /// Info log level
        /// </summary>
        private const string LogInfo = "INFO";

        /// <summary>
        /// Warn log level
        /// </summary>
        private const string LogWarn = "WARN";

        /// <summary>
        /// Error log level
        /// </summary>
        private const string LogError = "ERROR";

        /// <summary>
        /// Fatal log level
        /// </summary>
        private const string LogFatal = "FATAL";

        /// <summary>
        /// Hashtable of log levels
        /// </summary>
        private System.Collections.Hashtable logLevels =
            new System.Collections.Hashtable();

        /// <summary>
        /// Name of the logger
        /// </summary>
        private string loggerName;

        /// <summary>
        /// Log level in configuration
        /// </summary>
        private string configLogLevel;

        /// <summary>
        /// Logging file path
        /// </summary>
        private string logFilePath;

        /// <summary>
        /// Configurable property. In `app.config` set setting `LOG.SIMPLE.ELAPSED` to <b>ON/OFF</b><br/>
        /// example: 
        /// <example>`<add key="LOG.SIMPLE.ELAPSED" value="ON" />`</example><br/>
        /// In case is not set is `OFF`.
        /// </summary>
        public string SdkElapsed { get; private set; }

        /// <summary>
        /// Constructor for file logger.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        public File(string name)
        {
            this.logLevels[LogFatal] = 5;
            this.logLevels[LogError] = 4;
            this.logLevels[LogWarn] = 3;
            this.logLevels[LogInfo] = 2;
            this.logLevels[LogDebug] = 1;

            this.loggerName = name;

            this.SdkElapsed = ConfigurationManager.AppSettings["LOG.SIMPLE.ELAPSED"];
            this.configLogLevel = ConfigurationManager.AppSettings["LOG.SIMPLE.LEVEL"];
            string logFile = ConfigurationManager.AppSettings["LOG.SIMPLE.FILE"];
            string logPath = ConfigurationManager.AppSettings["LOG.SIMPLE.PATH"];
            this.logFilePath = logPath + "\\" + logFile;

            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            this.logFilePath = new Uri(Path.Combine(logPath, logFile)).LocalPath;
        }

        /// <summary>
        /// Log a debug level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Debug(string message)
        {
            this.ProcessMessage(message, LogDebug, null);
        }

        /// <summary>
        /// Log a debug level message and exception.
        /// </summary>
        /// <param name="message">Mesage to log</param>
        /// <param name="e">Exception to log</param>
        public void Debug(string message, Exception e)
        {
            this.ProcessMessage(message, LogDebug, e);
        }

        /// <summary>
        /// Log an info level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Info(string message)
        {
            this.ProcessMessage(message, LogInfo, null);
        }

        /// <summary>
        /// Log an info level message and exception.
        /// </summary>
        /// <param name="message">Mesage to log</param>
        /// <param name="e">Exception to log</param>
        public void Info(string message, Exception e)
        {
            this.ProcessMessage(message, LogInfo, e);
        }

        /// <summary>
        /// Log a warn level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Warn(string message)
        {
            this.ProcessMessage(message, LogWarn, null);
        }

        /// <summary>
        /// Log a warn level message and exception.
        /// </summary>
        /// <param name="message">Mesage to log</param>
        /// <param name="e">Exception to log</param>
        public void Warn(string message, Exception e)
        {
            this.ProcessMessage(message, LogWarn, e);
        }

        /// <summary>
        /// Log an error level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Error(string message)
        {
            this.ProcessMessage(message, LogError, null);
        }

        /// <summary>
        /// Log an error level message and exception.
        /// </summary>
        /// <param name="message">Mesage to log</param>
        /// <param name="e">Exception to log</param>
        public void Error(string message, Exception e)
        {
            this.ProcessMessage(message, LogError, e);
        }

        /// <summary>
        /// Log a fatal level message.
        /// </summary>
        /// <param name="message">Message to log</param>
        public void Fatal(string message)
        {
            this.ProcessMessage(message, LogFatal, null);
        }

        /// <summary>
        /// Log a fatal level message and exception.
        /// </summary>
        /// <param name="message">Mesage to log</param>
        /// <param name="e">Exception to log</param>
        public void Fatal(string message, Exception e)
        {
            this.ProcessMessage(message, LogFatal, e);
        }

        /// <summary>
        /// Process a log message, by determining if it should be logged,
        /// formatting the message, and logging it.
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">Logging level</param>
        /// <param name="e">Exception to log</param>
        protected void ProcessMessage(
            string message,
            string level,
            Exception e)
        {
            if (this.IsLoggable(level))
            {
                message = this.FormatMessage(message, level, e);
                this.Log(message);
            }
        }

        /// <summary>
        /// Format a message so it can be logged
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">Logging level</param>
        /// <param name="e">Exception to log</param>
        /// <returns>Formatted message</returns>
        protected string FormatMessage(
            string message,
            string level,
            Exception e)
        {
            DateTime date = DateTime.Now;
            string format = "yyyy-MM-ddTHH:mm:ss.fffzzz";
            string output = date.ToString(format) + " [" + level + "] " +
                this.loggerName + " - " + message + "\r\n";

            // Make sure exception is not null
            if (e != null)
            {
                output = output + e.ToString() + "\r\n";
                output = output + e.StackTrace + "\r\n";
            }

            return output;
        }

        /// <summary>
        /// Log a message.
        /// </summary>
        /// <param name="message">Message to log</param>
        protected void Log(string message)
        {
            DateTime date = DateTime.Now;
            string file = this.logFilePath + "." + date.ToString("yyyy-MM-dd");
            using (FileStream fs = new FileStream(file, FileMode.Append))
            {
                using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
                {
                    w.Write(message);
                }
            }
        }

        /// <summary>
        /// Determine if a message should be logged based on the logging
        /// level defined in the app configuration.
        /// </summary>
        /// <param name="level">Logging level of the message</param>
        /// <returns>True if the message should be logged, false otherwise</returns>
        protected bool IsLoggable(string level)
        {
            int configLevel = (int)this.logLevels[this.configLogLevel];
            int methodLevel = (int)this.logLevels[level];
            if (methodLevel >= configLevel)
            {
                return true;
            }

            return false;
        }
    }
}