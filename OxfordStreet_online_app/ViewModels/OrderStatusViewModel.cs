using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using OxfordStreet_online_app.Models;

namespace OxfordStreet_online_app.ViewModels
{
    public class OrderStatusViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Select a Items")]
        public OrderStatus OrderStatus { get; set; }
    }
}