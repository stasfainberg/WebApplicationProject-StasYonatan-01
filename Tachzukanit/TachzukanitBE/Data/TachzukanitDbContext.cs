using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TachzukanitBE.Models;

namespace TachzukanitBE.Data
{
    public class TachzukanitDbContext : IdentityDbContext
    {
        public DbSet<Malfunction> Malfunction { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Apartment> Apartment { get; set; }


        public TachzukanitDbContext(DbContextOptions<TachzukanitDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
