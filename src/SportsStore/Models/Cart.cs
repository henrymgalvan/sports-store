using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class Cart
    {

        private List<CartLine> _lineCollection = new List<CartLine>();

        public virtual void AddItem(Product product, int quantity)
        {
            CartLine cartLine = _lineCollection.Where(p => p.Product.ProductID == product.ProductID).FirstOrDefault();

            if (cartLine == null)
            {
                _lineCollection.Add(new CartLine { Product = product, Quantity = quantity });
            }
            else
            {
                cartLine.Quantity += quantity;
            }
        }

        public virtual void RemoveLine(Product product)
        {
            _lineCollection.RemoveAll(p => p.Product.ProductID == product.ProductID);
        }

        public virtual decimal ComputeValue() => _lineCollection.Sum(p => p.Product.Price * p.Quantity);

        public virtual IEnumerable<CartLine> Lines => _lineCollection;

        public virtual void Clear() => _lineCollection.Clear();
    }

    public class CartLine
    {
        [Key]
        public int CartId { get; set; }
        public Product @Product { get; set; }
        public int Quantity { get; set; }
    }

}
