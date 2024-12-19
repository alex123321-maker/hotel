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
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public string AdminId { get; set; }
        public int? DiscountId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual CustomerModel Customer { get; set; }
        public virtual RoomModel Room { get; set; }
        public virtual DiscountModel Discount { get; set; }
        public virtual StatusModel Status { get; set; }
        public virtual ICollection<ReservationServiceModel> ReservationServices { get; set; }
        public virtual ApplicationUser Admin { get; set; }

        public ReservationModel()
        {
            CreatedDate = DateTime.Now; // Устанавливаем текущую дату и время при создании объекта
        }
    }

}