//-----------------------------------------------------------------------
// <copyright file="InquiryTypes.cs" company="Keynetics Inc">
//     Copyright Kount. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Enums
{
    /// <summary>
    /// Inquiry type should be used for initial registration of the purchase in the Kount system.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>,<br/>
    /// <b>Version:</b> 8.0.0. <br/>
    /// <b>Copyright:</b> 2025 Equifax<br/>
    /// </summary>
    public enum InquiryTypes  
    {
        /// <summary>
        /// Default inquiry mode, internet order type
        /// </summary>
        ModeQ = 'Q',

        /// <summary>
        /// Used to analyze a phone-received order
        /// </summary>
        ModeP = 'P',

        /// <summary>
        /// Kount Central full inquiry with returned thresholds
        /// </summary>
        ModeW = 'W',

        /// <summary>
        /// Kount Central fast inquiry with just thresholds
        /// </summary>
        ModeJ = 'J',
    }
}