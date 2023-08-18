using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace hotel.Models
{
    public class ModelsContext:DbContext
    {
        public DbSet<CustomerModels> Customers { get; set; }

    }
}