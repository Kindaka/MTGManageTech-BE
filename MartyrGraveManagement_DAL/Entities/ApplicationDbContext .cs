using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MartyrGraveManagement_DAL.Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<FeedbackResponse> FeedbackResponses { get; set; }
        public DbSet<MartyrGrave> MartyrGraves { get; set; }
        public DbSet<MartyrGraveInformation> MartyrGraveInformations { get; set; }
        public DbSet<WeeklyReportGrave> WeeklyReportGraves { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<StaffTask> Tasks { get; set; }
        public DbSet<WorkPerformance> WorkPerformances { get; set; }
        public DbSet<StaffJob> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ServiceCategory Configuration
            modelBuilder.Entity<ServiceCategory>()
                .HasKey(sc => sc.CategoryId);

            // Service Configuration
            modelBuilder.Entity<Service>()
                .HasKey(s => s.ServiceId);
            modelBuilder.Entity<Service>()
                .HasOne(s => s.ServiceCategory)
                .WithMany(sc => sc.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // Account Configuration
            modelBuilder.Entity<Account>()
                .HasKey(a => a.AccountId);
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Role Configuration
            modelBuilder.Entity<Role>()
                .HasKey(r => r.RoleId);

            // Order Configuration
            modelBuilder.Entity<Order>()
                .HasKey(o => o.OrderId);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Account)
                .WithMany(a => a.Orders)
                .HasForeignKey(o => o.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderDetail Configuration
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => od.DetailId);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Service)
                .WithMany(s => s.OrderDetails)
                .HasForeignKey(od => od.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.MartyrGrave)
                .WithMany(mg => mg.OrderDetails)
                .HasForeignKey(od => od.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment Configuration
            modelBuilder.Entity<Payment>()
                .HasKey(p => p.PaymentId);
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem Configuration
            modelBuilder.Entity<CartItem>()
                .HasKey(ci => ci.CartId);
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Account)
                .WithMany(a => a.CartItems)
                .HasForeignKey(ci => ci.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Service)
                .WithMany(s => s.CartItems)
                .HasForeignKey(ci => ci.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.MartyrGrave)
                .WithMany(mg => mg.CartItems)
                .HasForeignKey(ci => ci.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);

            // Feedback Configuration
            modelBuilder.Entity<Feedback>()
                .HasKey(f => f.FeedbackId);
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Account)
                .WithMany(a => a.Feedbacks)
                .HasForeignKey(f => f.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Order)
                .WithOne(o => o.Feedback)
                .HasForeignKey<Feedback>(f => f.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // FeedbackResponse Configuration
            modelBuilder.Entity<FeedbackResponse>()
                .HasKey(fr => fr.Id);
            modelBuilder.Entity<FeedbackResponse>()
                .HasOne(fr => fr.Account)
                .WithMany(a => a.FeedbackResponses)
                .HasForeignKey(fr => fr.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // MartyrGrave Configuration
            modelBuilder.Entity<MartyrGrave>()
                .HasKey(mg => mg.MartyrId);
            modelBuilder.Entity<MartyrGrave>()
                .HasOne(mg => mg.Area)
                .WithMany(a => a.MartyrGraves)
                .HasForeignKey(mg => mg.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // MartyrGraveInformation Configuration
            modelBuilder.Entity<MartyrGraveInformation>()
                .HasKey(mgi => mgi.InformationId);
            modelBuilder.Entity<MartyrGraveInformation>()
                .HasOne(mgi => mgi.MartyrGrave)
                .WithMany(mg => mg.MartyrGraveInformations)
                .HasForeignKey(mgi => mgi.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);

            // WeeklyReportGrave Configuration
            modelBuilder.Entity<WeeklyReportGrave>()
                .HasKey(wrg => wrg.WeeklyReportId);
            modelBuilder.Entity<WeeklyReportGrave>()
                .HasOne(wrg => wrg.MartyrGrave)
                .WithMany(mg => mg.WeeklyReportGraves)
                .HasForeignKey(wrg => wrg.MartyrId)
                .OnDelete(DeleteBehavior.Restrict); 


            // Area Configuration
            modelBuilder.Entity<Area>()
                .HasKey(a => a.AreaId);

            // Task Configuration
            modelBuilder.Entity<StaffTask>()
                .HasKey(t => t.TaskId);
            modelBuilder.Entity<StaffTask>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StaffTask>()
                .HasOne(t => t.Order)
                .WithOne(o => o.Task)
                .HasForeignKey<StaffTask>(t => t.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkPerformance Configuration
            modelBuilder.Entity<WorkPerformance>()
                .HasKey(wp => wp.WorkId);
            modelBuilder.Entity<WorkPerformance>()
                .HasOne(wp => wp.Account)
                .WithMany(a => a.WorkPerformances)
                .HasForeignKey(wp => wp.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // StaffJob Configuration
            modelBuilder.Entity<StaffJob>()
                .HasKey(t => t.JobId);
            modelBuilder.Entity<StaffJob>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Jobs)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
