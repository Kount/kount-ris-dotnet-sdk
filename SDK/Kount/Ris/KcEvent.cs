//-----------------------------------------------------------------------
// <copyright file="KcEvent.cs" company="Keynetics Inc">
//     Copyright 2014 Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    /// <summary>
    /// Kount Central Event class description <br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2014 Keynetics Inc <br/>
    /// </summary>
    public class KcEvent
    {
        /// <summary>
        /// Get the code of this event
        /// </summary>
        private string code;

        /// <summary>
        /// Get the expression of this event
        /// </summary>
        private string expression;

        /// <summary>
        /// Get the decision of this event
        /// </summary>
        private string decision;

        /// <summary>
        /// A helper class to oraganize Kount Central threshold event data in the RIS response.
        /// </summary>
        /// <param name="code">The threshold event's code</param>
        /// <param name="expression">The threshold event's expression</param>
        /// <param name="decision">The threshold event's decision</param>
        public KcEvent(
          string code,
          string expression,
          string decision)
        {
            this.code = code;
            this.expression = expression;
            this.decision = decision;
        }

        /// <summary>
        /// Gets the threshold event code
        /// </summary>
        public string Code
        {
            get { return this.code; }
        }

        /// <summary>
        /// Gets the threshold event expression
        /// </summary>
        public string Expression
        {
            get { return this.expression; }
        }

        /// <summary>
        /// Gets the threshold event decision
        /// </summary>
        public string Decision
        {
            get { return this.decision; }
        }

        /// <summary>
        /// Our ToString method
        /// </summary>
        /// <returns>
        /// This object string representation
        /// </returns>
        public override string ToString()
        {
            return $"Code: {this.code}\n Expression: {this.expression}\n Decision: {this.decision}";
        }
    }
}