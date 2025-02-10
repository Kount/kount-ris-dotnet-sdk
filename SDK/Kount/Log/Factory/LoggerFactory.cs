namespace KountRisSdk.Kount.Log.Factory
{
    using Kount.Log.Binding;

    /// <summary>
    /// Interface for a logger factory.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Get a logger binding.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>A Kount.Log.Binding.Logger</returns>
        ILoggerSdk GetLogger(string name);
    }
}
