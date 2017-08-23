//-----------------------------------------------------------------------
// <copyright file="EnumHelper.cs" company="Kount Inc">
//     Copyright Kount. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Extention Helper
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>,<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2017 Kount Inc <br/>
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Extend functionality of PaymentTypes enum
        /// </summary>
        /// <param name="paymentType">PaymentTypes enum</param>
        /// <returns>Value definition in Description attribute</returns>
        public static string GetValueAsString(this Enums.PaymentTypes paymentType)
        {
            // get the field
            var field = paymentType.GetType().GetField(paymentType.ToString());
            var customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (customAttributes.Length > 0)
            {
                return (customAttributes[0] as DescriptionAttribute).Description;
            }
            else
            {
                return paymentType.ToString();
            }
        }

    }
}