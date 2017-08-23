//-----------------------------------------------------------------------
// <copyright file="PaymentTypes.cs" company="Kount Inc">
//     Copyright Kount. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// List of accepted payment types<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2017 Kount Inc <br/>
    /// </summary>
    public enum PaymentTypes
    {
        /// <summary>
        /// Apple payment type
        /// </summary>
        [Description("APAY")]
        Apple,

        /// <summary>
        /// Bill me later type
        /// </summary>
        [Description("BLML")]
        Blml,

        /// <summary>
        /// BPay payment type
        /// </summary>
        [Description("BPAY")]
        Bpay,

        /// <summary>
        /// Credit card type
        /// </summary>
        [Description("CARD")]
        Card,

        /// <summary>
        /// Carte Bleue payment type
        /// </summary>
        [Description("CARTE_BLEUE")]
        CarteBleue,

        /// <summary>
        /// The check type
        /// </summary>
        [Description("CHEK")]
        Check,

        /// <summary>
        /// ELV payment type
        /// </summary>
        [Description("ELV")]
        Elv,

        /// <summary>
        /// Green Dot MoneyPak payment type
        /// </summary>
        [Description("GDMP")]
        GreenDotMoneyPak,

        /// <summary>
        /// Gift card payment type
        /// </summary>
        [Description("GIFT")]
        GiftCard,

        /// <summary>
        /// GiroPay payment type
        /// </summary>
        [Description("GIROPAY")]
        GiroPay,

        /// <summary>
        /// Google payment type
        /// </summary>
        [Description("GOOG")]
        Google,

        /// <summary>
        /// Interac payment type
        /// </summary>
        [Description("INTERAC")]
        Interac,

        /// <summary>
        /// Mercade Pago payment type
        /// </summary>
        [Description("MERCADE_PAGO")]
        MercadePago,

        /// <summary>
        /// Neteller payment type
        /// </summary>
        [Description("NETELLER")]
        Neteller,

        /// <summary>
        /// No payment type
        /// </summary>
        [Description("NONE")]
        None,

        /// <summary>
        /// Poli payment type
        /// </summary>
        [Description("POLI")]
        Poli,

        /// <summary>
        /// Paypal payment type
        /// </summary>
        [Description("PYPL")]
        Paypal,

        /// <summary>
        /// Single Euro Payments Area type
        /// </summary>
        [Description("SEPA")]
        SingleEuroPaymentsArea,

        /// <summary>
        /// Skrill/Moneybookers payment type
        /// </summary>
        [Description("SKRILL")]
        Skrill,

        /// <summary>
        /// Sofort payment type
        /// </summary>
        [Description("SOFORT")]
        Sofort,

        /// <summary>
        /// Token payment type
        /// </summary>
        [Description("TOKEN")]
        Token
    }

}