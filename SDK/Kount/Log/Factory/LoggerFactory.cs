//-----------------------------------------------------------------------
// <copyright file="LoggerFactory.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log.Factory
{
    using Kount.Log.Binding;

    /// <summary>
    /// Interface for a logger factory.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Get a logger binding.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>A Kount.Log.Binding.Logger</returns>
        ILogger GetLogger(string name);
    }
}