//-----------------------------------------------------------------------
// <copyright file="UpdateType.cs" company="Keynetics Inc">
//     Copyright Kount. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    /// <summary>
    /// Update type should be used whenever there are changes to a given order and the merchant 
    /// wants them reflected into the Kount system.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 6.5.0. <br/>
    /// <b>Copyright:</b> 2017 Kount Inc <br/>
    /// </summary>
    public static class UpdateType
    {
        /// <summary>
        /// Default update mode, only sends the update event
        /// </summary>
        public const char ModeU = 'U';

        /// <summary>
        /// Sends the update event and RIS service returns a status response
        /// </summary>
        public const char ModeX = 'X';
    }
}