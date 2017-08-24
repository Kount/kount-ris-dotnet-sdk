//-----------------------------------------------------------------------
// <copyright file="CartItem.cs" company="Keynetics Inc">
//     Copyright Keynetics. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Kount.Ris
{
    /// <summary>
    /// Shopping cart item data object.<br/>
    /// <b>Author:</b> Kount <a>custserv@kount.com</a>;<br/>
    /// <b>Version:</b> 7.0.0. <br/>
    /// <b>Copyright:</b> 2010 Keynetics Inc <br/>
    /// </summary>
    public class CartItem
    {
        /// <summary>
        /// Product type or category of the item
        /// </summary>
        private string type;

        /// <summary>
        /// Name or SKU of the item
        /// </summary>
        private string item;

        /// <summary>
        /// Price of the item
        /// </summary>
        private long price;

        /// <summary>
        /// Description of the item
        /// </summary>
        private string description;

        /// <summary>
        /// Quantity of the item
        /// </summary>
        private long quantity;

        /// <summary>
        /// Cart item constructor
        /// </summary>
        /// <param name="productType">Product type as a 1 -255 char string</param>
        /// <param name="productItem">Product item as a 1 - 255 char string</param>
        /// <param name="productDescription">Product description as a 0 - 255 char string</param>
        /// <param name="productQuantity">Product quantity</param>
        /// <param name="productPrice">Product price</param>
        public CartItem(
            string productType,
            string productItem,
            string productDescription,
            long productQuantity,
            long productPrice)
        {
            this.ProductType = productType;
            this.ProductItem = productItem;
            this.ProductDescription = productDescription;
            this.ProductQuantity = productQuantity;
            this.ProductPrice = productPrice;
        }

        /// <summary>
        /// Gets or sets product type
        /// </summary>
        public string ProductType
        {
            get { return this.type; }
            set { this.type = value; }
        }

        /// <summary>
        /// Gets or sets the product item
        /// </summary>
        public string ProductItem
        {
            get { return this.item; }
            set { this.item = value; }
        }

        /// <summary>
        /// Gets or sets the product description
        /// </summary>
        public string ProductDescription
        {
            get { return this.description; }
            set { this.description = value; }
        }

        /// <summary>
        /// Gets or sets the product quantity
        /// </summary>
        public long ProductQuantity
        {
            get { return this.quantity; }
            set { this.quantity = value; }
        }

        /// <summary>
        /// Gets or sets the product price
        /// </summary>
        public long ProductPrice
        {
            get { return this.price; }
            set { this.price = value; }
        }
    }
}