using EventCatalogAPI.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventCatalogAPI.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<EventFormat> EventFormats { get; set; }
        public DbSet<EventItem> EventItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventCategory>(e =>
            {
                e.ToTable("EventCategories");
                e.Property(c => c.Id)
                .IsRequired()
                .UseHiLo("event_category_hilo");

                e.Property(c => c.Category)
                .IsRequired()
                .HasMaxLength(100);
            });

            modelBuilder.Entity<EventFormat>(e =>
            {
                e.ToTable("EventFormats");
                e.Property(f => f.Id)
                    .IsRequired()
                    .UseHiLo("event_format_hilo");

                e.Property(f => f.Format)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<EventItem>(e =>
            {
                e.ToTable("Event");
                e.Property(e => e.Id)
                    .IsRequired()
                    .UseHiLo("event_item_hilo");

                e.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                e.Property(e => e.Price)
                    .IsRequired();

                e.Property(e => e.PictureUrl)
                    .IsRequired();

                e.Property(e => e.DateAndTime)
                    .IsRequired();

                e.HasOne(e => e.EventFormat)
                    .WithMany()
                    .HasForeignKey(e => e.EventFormatId);

                e.HasOne(e => e.EventCategory)
                    .WithMany()
                    .HasForeignKey(e => e.EventCategoryId);
            });
        }
    }
}
