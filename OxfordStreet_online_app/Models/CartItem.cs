using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class CartItem
    {
        [Key]
        public int ItemId { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }//for LazyLoading

    }
}
