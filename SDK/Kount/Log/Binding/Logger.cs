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
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public interface ILoggerSdk : ILogger
    {
        bool MeasureElapsed { get; }
    }
}
