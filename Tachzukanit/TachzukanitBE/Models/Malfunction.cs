using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TachzukanitBE.Models
{
    public class Malfunction
    {
        #region Properties

        public string MalfunctionId { get; set; }

        public Status Status { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(25)]
        public string Title { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(200)]
        public string Content { get; set; }

        [DisplayName("Photo/Video")]
        public string Resources { get; set; }

        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Modification Date")]
        public DateTime ModifiedDate { get; set; }

        [DisplayName("Apartment")]
        public Apartment CurrentApartment { get; set; }

        [DisplayName("Requested By")]
        public User RequestedBy { get; set; }

        #endregion
    }
}
