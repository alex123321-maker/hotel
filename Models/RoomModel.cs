using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace hotel.Models
{
    public class RoomModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RoomTypeId { get; set; }
        public int CapacityId { get; set; }
        public decimal Price { get; set; }

        public virtual RoomTypeModel RoomType { get; set; }
        public virtual RoomCapacityModel Capacity { get; set; }
    }
}