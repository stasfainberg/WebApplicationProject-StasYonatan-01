using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tachzukanit.Models
{
    public class Apartment
    {
        #region Properties

        public int ApartmentId { get; set; }

        public string Address { get; set; }

        public string Photo { get; set; }

        public string RoomsNumber { get; set; }

        public ICollection<Malfunction> malfunctions { get; set; }

        #endregion
    }
}
