//-----------------------------------------------------------------------
// <copyright file="ValidationError.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    /// <summary>
    /// A class representing a RIS SDK client side validation error.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Missing required field validation error
        /// </summary>
        private const string RequiredError = "REQUIRED";

        /// <summary>
        /// Regular expression validation error
        /// </summary>
        private const string RegexError = "REGEX";

        /// <summary>
        /// Maximum length exceeded validation error
        /// </summary>
        private const string LengthError = "LENGTH";

        /// <summary>
        /// The type of error this is.
        /// </summary>
        private string error;

        /// <summary>
        /// The field.
        /// </summary>
        private string field;

        /// <summary>
        /// The RIS mode.
        /// </summary>
        private string mode;

        /// <summary>
        /// Field value.
        /// </summary>
        private string value;

        /// <summary>
        /// Regular expression pattern.
        /// </summary>
        private string pattern;

        /// <summary>
        /// Maximum allowable length of a field.
        /// </summary>
        private int maxLength;

        /// <summary>
        /// Constructor for missing required field.
        /// </summary>
        /// <param name="field">The name of the bad field</param>
        /// <param name="mode">The RIS mode the field is associated with</param>
        public ValidationError(string field, string mode)
        {
            this.error = RequiredError;
            this.field = field;
            this.mode = mode;
        }

        /// <summary>
        /// Constructor for regular expression error.
        /// </summary>
        /// <param name="field">The name of the bad field</param>
        /// <param name="value">The value of the field</param>
        /// <param name="pattern">The regular expression violated</param>
        public ValidationError(string field, string value, string pattern)
        {
            this.error = RegexError;
            this.field = field;
            this.value = value;
            this.pattern = pattern;
        }

        /// <summary>
        /// Constructor for maximum length error.
        /// </summary>
        /// <param name="field">The name of the bad field</param>
        /// <param name="value">The value of the field</param>
        /// <param name="length">The maximum allowable length</param>
        public ValidationError(string field, string value, int length)
        {
            this.error = LengthError;
            this.field = field;
            this.value = value;
            this.maxLength = length;
        }

        /// <summary>
        /// Get the string representation of the error.
        /// </summary>
        /// <returns>Error message string</returns>
        public override string ToString()
        {
            if (this.error == LengthError)
            {
                return $"Field [{this.field}] has length [{this.value.Length}] which is longer than the maximum of [{this.maxLength}]";
            }
            else if (this.error == RegexError)
            {
                return $"Field [{this.field}] has value [{this.value}] which does not match the pattern [{this.pattern}]";
            }
            else if (this.error == RequiredError)
            {
                return $"Required field [{this.field}] missing for mode [{this.mode}]";
            }

            return null;
        }
    }
}