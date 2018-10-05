using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tachzukanit.Models
{
    public class TachzukanitContext : DbContext
    {
        public TachzukanitContext (DbContextOptions<TachzukanitContext> options)
            : base(options)
        {
        }

        public DbSet<Tachzukanit.Models.Malfunction> Malfunction { get; set; }
    }
}
