using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Tachzukanit.Models
{
    public class User
    {
        #region Properties

        public string UserId { get; set; }

        [StringLength(25, MinimumLength = 2)]
        [Required]
        public string Name { get; set; }

        [Required]
        public Role Role { get; set; }

        [Required]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        public string FacebookId { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{4})$", ErrorMessage = "The Phone Number field is not a valid phone number")]
        public string Phone { get; set; }

        [StringLength(50, MinimumLength = 8)]
        [Required]
        public string Address { get; set; }

        public ICollection<Malfunction> malfunctions { get; set; }

        #endregion
    }
}
