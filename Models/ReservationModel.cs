using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace hotel.Models
{
    public class ReservationModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Customer")]
        public string CustomerId { get; set; }
        public virtual ApplicationUser Customer { get; set; }

        [ForeignKey("Room")]
        public int RoomNumber { get; set; }
        public virtual RoomModel Room { get; set; }

        [ForeignKey("Admin")]
        public string AdminId { get; set; }
        public virtual ApplicationUser Admin { get; set; }

        [ForeignKey("Discount")]
        public int? DiscountId { get; set; } // Nullable, as there might be no discount
        public virtual DiscountModel Discount { get; set; }

        [ForeignKey("Status")]
        public string StatusId { get; set; }
        public virtual StatusModel Status { get; set; }

        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}