using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tachzukanit.Models
{
    public class Malfunction
    {

        #region Properties

        public int Id { get; set; }

        public int Status { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Resource { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        #endregion
    }
}
