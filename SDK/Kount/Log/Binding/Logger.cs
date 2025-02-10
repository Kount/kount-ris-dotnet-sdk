using System;
using System.Collections.Generic;
using System.Text;

namespace KountRisSdk.Kount.Log.Binding
{
    using Microsoft.Extensions.Logging;
    using System;

    /// <summary>
    /// Logger interface.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public interface ILoggerSdk : ILogger
    {
        /// <summary>
        /// Whether to measure elapsed time
        /// </summary>
        bool MeasureElapsed { get; }
    }
}
