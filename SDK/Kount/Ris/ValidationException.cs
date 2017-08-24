//-----------------------------------------------------------------------
// <copyright file="ValidationException.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    using System;

    /// <summary>
    /// Validation Exception.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Create an exception with an error message
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public ValidationException(string errorMessage) : base(errorMessage)
        {
        }

        /// <summary>
        /// Create an exception with an error message and another exception
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerEx">Inner exception.</param>
        public ValidationException(string errorMessage, Exception innerEx)
            : base(errorMessage, innerEx)
        {
        }

        /// <summary>
        /// Gets a string of the error message.
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