using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        //RELATIONS
        public Product Product { get; set; }//foreign key
        public Order Order { get; set; }//foreign key
    }
}