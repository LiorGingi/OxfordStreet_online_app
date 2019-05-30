using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Total cost should be greater than or equal to 0.")]
        [Required]
        public int TotalCost { get; set; }

        [Required]
        public string Status { get; set; }

        //RELATIONS
        public User User { get; set; }//foreign key
        public Branch Branch { get; set; }//foreign key
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}