using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tachzukanit.Models
{
    public class Apartment
    {
        #region Properties

        public int ApartmentId { get; set; }

        [StringLength(50, MinimumLength = 8)]
        [Required]
        public string Address { get; set; }

        public string Photo { get; set; }

        [Required]
        [Range(1 , 10)]
        public string RoomsNumber { get; set; }

        public ICollection<Malfunction> malfunctions { get; set; }

        #endregion
    }
}
