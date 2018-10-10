using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Tachzukanit.Models
{
    public class TachzukanitContext : DbContext
    {
        public DbSet<Malfunction> Malfunction { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Apartment> Apartment { get; set; }


        public TachzukanitContext (DbContextOptions<TachzukanitContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Malfunction>()
                .HasOne(mal => mal.RequestedBy)                
                .WithMany(usr => usr.malfunctions)
                .IsRequired();

            modelBuilder.Entity<Malfunction>()
                .HasOne(mal => mal.CurrentApartment)
                .WithMany(apt => apt.malfunctions)
                .IsRequired();

        }
    }
}
