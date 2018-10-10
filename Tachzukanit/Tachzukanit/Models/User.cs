﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tachzukanit.Models
{
    public class User
    {
        #region Properties

        public string UserId { get; set; }

        public string Name { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public string FacebookId { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public ICollection<Malfunction> malfunctions { get; set; }

        #endregion
    }
}
