using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hotel.Models
{
    public class DiscountModel
    {
        [Key]
        public int Id { get; set; }
        public int Days{ get; set; }
        public int Amount { get; set; }

    }
}