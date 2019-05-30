using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Price should be greater than or equal to 0.")]
        [Required]
        public int Price { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string SubCategory { get; set; }

        [Required]
        public string WeatherType { get; set; }

        [Required]
        public int SupplierId { get; set; }

        //RELATIONS
        public Supplier Supplier { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}