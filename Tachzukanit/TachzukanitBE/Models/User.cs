using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TachzukanitBE.Models
{
    public class User : IdentityUser
    {
        #region Properties

        public string UserId { get; set; }

        public string FullName { get; set; }
        
        public string FacebookId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public ICollection<Malfunction> malfunctions { get; set; }

        #endregion
    }
}
