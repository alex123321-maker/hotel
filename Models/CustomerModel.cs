using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hotel.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id{ get; set; } 
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Passportseries { get; set; }
        public int Passportnumber { get; set; }
        public string Prefers{ get; set; }

    }
}