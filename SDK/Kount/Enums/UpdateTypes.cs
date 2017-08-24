//-----------------------------------------------------------------------
// <copyright file="InquiryTypes.cs" company="Keynetics Inc">
//     Copyright Kount. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Enums
{
    /// <summary>
    /// Update type should be used whenever there are changes to a given order and the merchant 
    /// wants them reflected into the Kount system.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2017 Kount Inc <br/>
    /// </summary>
    public enum UpdateTypes
    {
        /// <summary>
        /// Default update mode, only sends the update event
        /// </summary>
        ModeU = 'U',

        /// <summary>
        /// Sends the update event and RIS service returns a status response
        /// </summary>
        ModeX = 'X'
    }
}
