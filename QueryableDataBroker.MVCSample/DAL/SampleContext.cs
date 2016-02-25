using Microsoft.Data.Entity;
using QueryableDataBroker.MVCSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryableDataBroker.MVCSample.DAL
{
    public interface ISampleContext
    {
        DbSet<Unicorn> Unicorns { get; set; }
    }

    public class SampleContext : DbContext
    {
        public DbSet<Unicorn> Unicorns { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseInMemoryDatabase();
            base.OnConfiguring(options);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var unicorns = modelBuilder.Entity<Unicorn>();

            unicorns.HasKey(u => u.Id);
            unicorns.Property(u => u.Name).IsRequired();
            unicorns.Ignore(u => u.Age);
        }
    }
}