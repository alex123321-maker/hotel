using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hotel.Models
{
    public class PricingModel
    {
        [Key]
        public string day_of_week { get; set; }
        public double multiplier { get; set; }

    }
}