//-----------------------------------------------------------------------
// <copyright file="SimpleLoggerFactory.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Log.Factory
{
    using Kount.Log.Binding;

    /// <summary>
    /// A simple logger binding class.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class SimpleLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Get a simple logger binding.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>A Kount.Log.Binding.SimpleLogger</returns>
        public ILogger GetLogger(string name)
        {
            return new Kount.Log.Binding.SimpleLogger(name);
        }
    }
}