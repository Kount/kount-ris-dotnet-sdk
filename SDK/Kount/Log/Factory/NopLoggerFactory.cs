namespace KountRisSdk.Kount.Log.Factory
{
    using Kount.Log.Binding;

    /// <summary>
    /// A NOP logger binding class.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public class NopLoggerFactory : ILoggerFactory
    {
        /// <summary>
        /// Get a NOP logger binding.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>A Kount.Log.Binding.NopLogger</returns>
        public ILoggerSdk GetLogger(string name)
        {
            return new NopLogger(name);
        }
    }
}
