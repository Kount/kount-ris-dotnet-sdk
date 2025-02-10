namespace KountRisSdk.Kount.Log.Factory
{
    using Kount.Log.Binding;

    /// <summary>
    /// A simple logger binding class.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public class SimpleLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Get a simple logger binding.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>A Kount.Log.Binding.SimpleLogger</returns>
        public ILoggerSdk GetLogger(string name)
        {
            return new Kount.Log.Binding.SimpleLogger(name);
        }
    }
}
