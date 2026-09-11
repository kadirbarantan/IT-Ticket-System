using ITTicketSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace ITTicketSystem.Data
{
    internal class AppDbContext:DbContext
    {
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Status> Statuses { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Donanım" },
                new Category { Id = 2, Name = "Yazılım" },
                new Category { Id = 3, Name = "Ağ" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { Id = 1, Name = "Açık" },
                new Status { Id = 2, Name = "İşlemde" },
                new Status { Id = 3, Name = "Kapalı" }
            );

            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, Name = "Ahmet Yılmaz", Department = "IT" },
                new Employee { Id = 2, Name = "Elif Demir", Department = "Muhasebe" }
            );
        }
    }
}