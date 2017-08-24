//-----------------------------------------------------------------------
// <copyright file="IllegalArgumentException.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    using System;

    /// <summary>
    /// Kount Ris Illegal argument exception.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class IllegalArgumentException : Exception
    {
        /// <summary>
        /// Create the exception with an error message
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public IllegalArgumentException(string errorMessage) :
            base(errorMessage)
        {
        }

        /// <summary>
        /// Create the exception with an error message and another exception.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerEx">Inner exception.</param>
        public IllegalArgumentException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx)
        {
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this.Message.ToString();
            }
        }
    }
}