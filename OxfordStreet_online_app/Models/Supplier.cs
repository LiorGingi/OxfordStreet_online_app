using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OxfordStreet_online_app.Models
{
    public class Supplier
    {
        public int SupplierId { get; set; }

        [DisplayName("Supplier Name")]
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$", ErrorMessage = "The Phone Number field is not a valid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }
    }
}