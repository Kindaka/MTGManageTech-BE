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
        public DbSet<Material> Materials { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CartItemCustomer> CartItems { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<MartyrGrave> MartyrGraves { get; set; }
        public DbSet<GraveImage> GraveImages { get; set; }
        public DbSet<MartyrGraveInformation> MartyrGraveInformations { get; set; }
        public DbSet<WeeklyReportGrave> WeeklyReportGraves { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<StaffTask> Tasks { get; set; }
        public DbSet<WorkPerformance> WorkPerformances { get; set; }
        public DbSet<GraveService> GraveServices { get; set; }
        public DbSet<HistoricalEvent> HistoricalEvents { get; set; }
        public DbSet<HistoricalImage> HistoricalImages { get; set; }
        public DbSet<HistoricalRelatedMartyr> HistoricalRelatedMartyrs { get; set; }
        public DbSet<Holiday_Event> HolidayEvents { get; set; }
        public DbSet<Icon> Icons { get; set; }
        public DbSet<Comment_Icon> Comment_Icons { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event_Image> EventImages { get; set; }
        public DbSet<Comment_Report> CommentReports { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Schedule_Staff> ScheduleStaffs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAccount> NotificationAccounts { get; set; }
        public DbSet<Slot> Slots { get; set; }

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
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // CartItem Configuration
            modelBuilder.Entity<CartItemCustomer>()
                .HasKey(ci => ci.CartId);
            modelBuilder.Entity<CartItemCustomer>()
                .HasOne(ci => ci.Account)
                .WithMany(a => a.CartItems)
                .HasForeignKey(ci => ci.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItemCustomer>()
                .HasOne(ci => ci.Service)
                .WithMany(s => s.CartItems)
                .HasForeignKey(ci => ci.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItemCustomer>()
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
                .HasOne(f => f.OrderDetail)
                .WithOne(o => o.Feedback)
                .HasForeignKey<Feedback>(f => f.DetailId)
                .OnDelete(DeleteBehavior.Restrict);


            // MartyrGrave Configuration
            modelBuilder.Entity<MartyrGrave>()
                .HasKey(mg => mg.MartyrId);
            modelBuilder.Entity<MartyrGrave>()
                .HasOne(mg => mg.Area)
                .WithMany(a => a.MartyrGraves)
                .HasForeignKey(mg => mg.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MartyrGrave>()
                .HasOne(mg => mg.Location)
                .WithOne(a => a.MartyrGraves)
                .HasForeignKey<MartyrGrave>(mg => mg.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<MartyrGrave>()
                .HasOne(mg => mg.Account)
                .WithMany(a => a.MartyrGraves)
                .HasForeignKey(mg => mg.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // MartyrGraveInformation Configuration
            modelBuilder.Entity<MartyrGraveInformation>()
                .HasKey(mgi => mgi.InformationId);
            modelBuilder.Entity<MartyrGraveInformation>()
                .HasOne(mgi => mgi.MartyrGrave)
                .WithMany(mg => mg.MartyrGraveInformations)
                .HasForeignKey(mgi => mgi.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);

            //GraveService Configuration
            modelBuilder.Entity<GraveService>()
                .HasKey(mgi => mgi.GraveServiceId);
            modelBuilder.Entity<GraveService>()
                .HasOne(mgi => mgi.MartyrGrave)
                .WithMany(mg => mg.GraveServices)
                .HasForeignKey(mgi => mgi.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<GraveService>()
                .HasOne(mgi => mgi.Service)
                .WithMany(mg => mg.GraveServices)
                .HasForeignKey(mgi => mgi.ServiceId)
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

            // Location Configuration
            modelBuilder.Entity<Location>()
                .HasKey(l => l.LocationId);

            //Historical_Event Configuration
            modelBuilder.Entity<HistoricalEvent>()
                .HasKey(he => he.HistoryId);

            // Historical_Related_Martyr Configuration
            modelBuilder.Entity<HistoricalRelatedMartyr>()
                .HasKey(hrm => hrm.RelatedId);
            modelBuilder.Entity<HistoricalRelatedMartyr>()
                .HasOne(hrm => hrm.MartyrGraveInformation)
                .WithMany(mg => mg.HistoricalRelatedMartyrs)
                .HasForeignKey(hrm => hrm.InformationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HistoricalRelatedMartyr>()
                .HasOne(hrm => hrm.History)
                .WithMany(hrm => hrm.HistoricalRelatedMartyrs)
                .HasForeignKey(hrm => hrm.HistoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Historical_Image Configuration
            modelBuilder.Entity<HistoricalImage>()
                .HasKey(hi => hi.ImageId);
            modelBuilder.Entity<HistoricalImage>()
                .HasOne(hi => hi.HistoricalEvent)
                .WithMany(hi => hi.HistoricalImages)
                .HasForeignKey(hi => hi.HistoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment Configuration
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.CommentId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.HistoricalEvent)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.HistoryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Account)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            //Icon Configuration
            modelBuilder.Entity<Icon>()
                .HasKey(i => i.IconId);

            //Comment_Icon Configuration
            modelBuilder.Entity<Comment_Icon>()
                .HasKey(ci => ci.Id);
            modelBuilder.Entity<Comment_Icon>()
                .HasOne(ci => ci.Icon)
                .WithMany(ci => ci.Comment_Icons)
                .HasForeignKey(ci => ci.IconId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comment_Icon>()
                .HasOne(ci => ci.Comment)
                .WithMany(ci => ci.Comment_Icons)
                .HasForeignKey(ci => ci.CommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Report Configuration
            modelBuilder.Entity<Comment_Report>()
                .HasKey(cr => cr.ReportId);
            modelBuilder.Entity<Comment_Report>()
                .HasOne(cr => cr.Comment)
                .WithMany(cr => cr.Comment_Reports)
                .HasForeignKey(cr => cr.CommentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comment_Report>()
                .HasOne(cr => cr.Account)
                .WithMany(cr => cr.Comment_Reports)
                .HasForeignKey(cr => cr.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Holiday_Event Configuration
            modelBuilder.Entity<Holiday_Event>()
                .HasKey(he => he.EventId);
            modelBuilder.Entity<Holiday_Event>()
                .HasOne(he => he.Account)
                .WithMany(he => he.Holiday_Events)
                .HasForeignKey(he => he.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event_Image Configuration
            modelBuilder.Entity<Event_Image>()
                .HasKey(ei => ei.ImageId);
            modelBuilder.Entity<Event_Image>()
                .HasOne(ei => ei.Holiday_Event)
                .WithMany(ei => ei.EventImages)
                .HasForeignKey(ei => ei.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task Configuration
            modelBuilder.Entity<StaffTask>()
                .HasKey(t => t.TaskId);
            modelBuilder.Entity<StaffTask>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Tasks)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<StaffTask>()
                .HasOne(t => t.OrderDetail)
                .WithMany(o => o.StaffTasks)
                .HasForeignKey(t => t.DetailId)
                .OnDelete(DeleteBehavior.Restrict);

            // Slot Configuration
            modelBuilder.Entity<Slot>()
                .HasKey(s => s.SlotId);

            // Schedule_Staff Configuration
            modelBuilder.Entity<Schedule_Staff>()
                .HasKey(ss => ss.ScheduleId);
            modelBuilder.Entity<Schedule_Staff>()
                .HasOne(ss => ss.Account)
                .WithMany(ss => ss.Schedules)
                .HasForeignKey(ss => ss.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Schedule_Staff>()
                .HasOne(ss => ss.Slot)
                .WithMany(ss => ss.Schedules)
                .HasForeignKey(ss => ss.SlotId)
                .OnDelete(DeleteBehavior.Restrict);

            //Notification Configuration
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            //Notification_Account Configuration
            modelBuilder.Entity<NotificationAccount>()
                .HasKey(n => n.Id);
            modelBuilder.Entity<NotificationAccount>()
                .HasOne(n => n.Notification)
                .WithMany(n => n.NotificationAccounts)
                .HasForeignKey(n => n.NotificationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NotificationAccount>()
                .HasOne(n => n.Account)
                .WithMany(n => n.NotificationAccounts)
                .HasForeignKey(n => n.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkPerformance Configuration
            modelBuilder.Entity<WorkPerformance>()
                .HasKey(wp => wp.WorkId);
            modelBuilder.Entity<WorkPerformance>()
                .HasOne(wp => wp.Account)
                .WithMany(a => a.WorkPerformances)
                .HasForeignKey(wp => wp.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
     

            // GraveImage Configuration
            modelBuilder.Entity<GraveImage>()
                .HasKey(t => t.ImageId);
            modelBuilder.Entity<GraveImage>()
                .HasOne(t => t.MartyrGrave)
                .WithMany(a => a.GraveImages)
                .HasForeignKey(t => t.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);

            // Material Configuration
            modelBuilder.Entity<Material>()
                .HasKey(t => t.MaterialId);
            modelBuilder.Entity<Material>()
                .HasOne(t => t.Service)
                .WithMany(a => a.Materials)
                .HasForeignKey(t => t.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
