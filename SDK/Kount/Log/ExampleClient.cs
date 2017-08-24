//-----------------------------------------------------------------------
// <copyright file="ExampleClient.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log
{
    using Kount.Log.Binding;
    using Kount.Log.Factory;
    using System;

    /// <summary>
    /// A class demonstrating how to use logging
    /// </summary>
    public class ExampleClient
    {
        /// <summary>
        /// The main entry point for the application.<br/>
        /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
        /// <b>Version:</b> 7.0.0. <br/>
        /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
        /// </summary>
        [STAThread]
        public static void Main()
        {
            ILoggerFactory factory = LogFactory.GetLoggerFactory();
            ILogger logger = factory.GetLogger("Example Client");
            logger.Debug("Hello World");
            logger.Info("Hello World");
            logger.Warn("Hello World");
            logger.Error("Hello World");
            logger.Fatal("Hello World");

            // How to log messages and exceptions.
            try
            {
                Exception e = new Exception("Logging an exception");
                throw e;
            }
            catch (Exception e)
            {
                logger.Debug("Hello World", e);
                logger.Info("Hello World", e);
                logger.Warn("Hello World", e);
                logger.Error("Hello World", e);
                logger.Fatal("Hello World", e);
            }
        }
    }
}