using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hotel.Models
{
    public class ReservationServiceModel
    {
        public int Id { get; set; }
        public int ReservationId { get; set; }
        public int ServiceId { get; set; }

        public bool Complited { get; set; }

        public virtual ReservationModel Reservation { get; set; }
        public virtual ServiceModel Service { get; set; }

        public ReservationServiceModel()
        {
            Complited = false;
        }
    }

}
