﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotel.Models
{
    public class PaymentModel
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }

        public virtual ReservationModel Reservation { get; set; }
    }

}
