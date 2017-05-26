//-----------------------------------------------------------------------
// <copyright file="InquiryType.cs" company="Keynetics Inc">
//     Copyright Kount. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    /// <summary>
    /// Inquiry type should be used for initial registration of the purchase in the Kount system.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 6.5.0. <br/>
    /// <b>Copyright:</b> 2017 Kount Inc <br/>
    /// </summary>
    public static class InquiryType
    {
        /// <summary>
        /// Default inquiry mode, internet order type
        /// <code>public const char ModeP = 'Q';</code>
        /// </summary>
        public const char ModeQ = 'Q';

        /// <summary>
        /// Used to analyze a phone-received order
        /// <code>public const char ModeP = 'P';</code>
        /// </summary>
        public const char ModeP = 'P';

        /// <summary>
        /// Kount Central full inquiry with returned thresholds
        /// <code>public const char ModeP = 'W';</code>
        /// </summary>
        public const char ModeW = 'W';

        /// <summary>
        /// Kount Central fast inquiry with just thresholds
        /// <code>public const char ModeP = 'J';</code>
        /// </summary>
        public const char ModeJ = 'J';
    }
}