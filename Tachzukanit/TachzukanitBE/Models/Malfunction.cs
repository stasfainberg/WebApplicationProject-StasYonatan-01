using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tachzukanit.Models
{
    public class Malfunction
    {
        #region Properties

        public string MalfunctionId { get; set; }

        public Status Status { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Resources { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public Apartment CurrentApartment { get; set; }

        public User RequestedBy { get; set; }

        #endregion
    }
}
