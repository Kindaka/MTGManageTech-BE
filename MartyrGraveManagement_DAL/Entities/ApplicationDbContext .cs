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
        public DbSet<ReportGrave> ReportGraves { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<StaffTask> Tasks { get; set; }
        public DbSet<WorkPerformance> WorkPerformances { get; set; }
        public DbSet<GraveService> GraveServices { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<HistoricalImage> HistoricalImages { get; set; }
        public DbSet<BlogCategory> BlogCategory { get; set; }
        public DbSet<HistoricalRelatedMartyr> HistoricalRelatedMartyrs { get; set; }
        public DbSet<Holiday_Event> HolidayEvents { get; set; }
        public DbSet<Icon> Icons { get; set; }
        public DbSet<Comment_Icon> Comment_Icons { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Event_Image> EventImages { get; set; }
        public DbSet<Comment_Report> CommentReports { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationAccount> NotificationAccounts { get; set; }
        //public DbSet<Slot> Slots { get; set; }
        public DbSet<ScheduleDetail> ScheduleTasks { get; set; }
        //public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Material_Service> Material_Services { get; set; }

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

            // CustomerWallet Configuration
            modelBuilder.Entity<CustomerWallet>()
                .HasKey(a => a.WalletId);
            modelBuilder.Entity<CustomerWallet>()
                .HasOne(a => a.Account)
                .WithOne(r => r.CustomerWallet)
                .HasForeignKey<CustomerWallet>(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransactionBalanceHistory Configuration
            modelBuilder.Entity<TransactionBalanceHistory>()
                .HasKey(a => a.TransactionId);
            modelBuilder.Entity<TransactionBalanceHistory>()
                .HasOne(a => a.Account)
                .WithMany(r => r.TransactionBalanceHistorys)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Service_Schedule Configuration
            modelBuilder.Entity<Service_Schedule>()
                .HasKey(a => a.ServiceScheduleId);
            modelBuilder.Entity<Service_Schedule>()
                .HasOne(a => a.Account)
                .WithMany(r => r.ServiceSchedules)
                .HasForeignKey(a => a.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Service_Schedule>()
                .HasOne(a => a.Service)
                .WithMany(r => r.ServiceSchedules)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Service_Schedule>()
                .HasOne(a => a.MartyrGrave)
                .WithMany(r => r.ServiceSchedules)
                .HasForeignKey(a => a.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);


            // AssignmentTask Configuration
            modelBuilder.Entity<AssignmentTask>()
                .HasKey(a => a.AssignmentTaskId);
            modelBuilder.Entity<AssignmentTask>()
                .HasOne(a => a.Service_Schedule)
                .WithMany(r => r.AssignmentTasks)
                .HasForeignKey(a => a.ServiceScheduleId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AssignmentTask>()
                .HasOne(a => a.Account)
                .WithMany(r => r.AssignmentTasks)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssignmentTaskImage Configuration
            modelBuilder.Entity<AssignmentTaskImage>()
                .HasKey(a => a.ImageId);
            modelBuilder.Entity<AssignmentTaskImage>()
                .HasOne(a => a.AssignmentTask)
                .WithMany(r => r.AssignmentTaskImages)
                .HasForeignKey(a => a.AssignmentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssignmentTask_Feedback Configuration
            modelBuilder.Entity<AssignmentTask_Feedback>()
                .HasKey(a => a.AssignmentTaskFeedbackId);
            modelBuilder.Entity<AssignmentTask_Feedback>()
                .HasOne(a => a.AssignmentTask)
                .WithOne(r => r.Feedback)
                .HasForeignKey<AssignmentTask_Feedback>(a => a.AssignmentTaskId)
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


            // RequestType Configuration
            modelBuilder.Entity<RequestType>()
                .HasKey(wrg => wrg.TypeId);

            // RequestCustomer Configuration
            modelBuilder.Entity<RequestCustomer>()
                .HasKey(wrg => wrg.RequestId);
            modelBuilder.Entity<RequestCustomer>()
                .HasOne(wrg => wrg.Account)
                .WithMany(mg => mg.RequestCustomers)
                .HasForeignKey(wrg => wrg.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequestCustomer>()
                .HasOne(wrg => wrg.MartyrGrave)
                .WithMany(mg => mg.RequestCustomers)
                .HasForeignKey(wrg => wrg.MartyrId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RequestCustomer>()
                .HasOne(wrg => wrg.RequestType)
                .WithMany(mg => mg.RequestCustomers)
                .HasForeignKey(wrg => wrg.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            //RequestImage Configuration
            modelBuilder.Entity<RequestImage>()
                .HasKey(rpi => rpi.RequestImageId);
            modelBuilder.Entity<RequestImage>()
                .HasOne(rpi => rpi.RequestCustomer)
                .WithMany(rpi => rpi.RequestImages)
                .HasForeignKey(rpi => rpi.RequestId);

            // ReportGrave Configuration
            modelBuilder.Entity<ReportGrave>()
                .HasKey(wrg => wrg.ReportId);
            modelBuilder.Entity<ReportGrave>()
                .HasOne(wrg => wrg.RequestCustomer)
                .WithOne(mg => mg.ReportGrave)
                .HasForeignKey<ReportGrave>(wrg => wrg.RequestId)
                .OnDelete(DeleteBehavior.Restrict);

            // ReportImage Configuration
            modelBuilder.Entity<ReportImage>()
                .HasKey(wrg => wrg.ImageId);
            modelBuilder.Entity<ReportImage>()
                .HasOne(wrg => wrg.ReportGrave)
                .WithMany(mg => mg.ReportImages)
                .HasForeignKey(wrg => wrg.ReportId)
                .OnDelete(DeleteBehavior.Restrict);


            // Area Configuration
            modelBuilder.Entity<Area>()
                .HasKey(a => a.AreaId);

            // Location Configuration
            modelBuilder.Entity<Location>()
                .HasKey(l => l.LocationId);

            //Historical_Event Configuration
            modelBuilder.Entity<Blog>()
                .HasKey(b => b.BlogId);
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Account)
                .WithMany(b => b.Blogs)
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Blog>()
                .HasOne(b => b.HistoricalEvent)
                .WithMany(b => b.Blogs)
                .HasForeignKey(b => b.HistoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Historical_Related_Martyr Configuration
            modelBuilder.Entity<HistoricalRelatedMartyr>()
                .HasKey(hrm => hrm.RelatedId);
            modelBuilder.Entity<HistoricalRelatedMartyr>()
                .HasOne(hrm => hrm.MartyrGraveInformation)
                .WithMany(mg => mg.HistoricalRelatedMartyrs)
                .HasForeignKey(hrm => hrm.InformationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<HistoricalRelatedMartyr>()
                .HasOne(hrm => hrm.Blog)
                .WithMany(hrm => hrm.HistoricalRelatedMartyrs)
                .HasForeignKey(hrm => hrm.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            // Historical_Image Configuration
            modelBuilder.Entity<HistoricalImage>()
                .HasKey(hi => hi.ImageId);
            modelBuilder.Entity<HistoricalImage>()
                .HasOne(hi => hi.Blog)
                .WithMany(hi => hi.HistoricalImages)
                .HasForeignKey(hi => hi.BlogId)
                .OnDelete(DeleteBehavior.Restrict);

            // Comment Configuration
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.CommentId);
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blog)
                .WithMany(c => c.Comments)
                .HasForeignKey(c => c.BlogId)
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
                .WithOne(o => o.StaffTask)
                .HasForeignKey<StaffTask>(t => t.DetailId)
                .OnDelete(DeleteBehavior.Restrict);

            //TaskImage Configuration
            modelBuilder.Entity<TaskImage>()
                .HasKey(t => t.ImageId);
            modelBuilder.Entity<TaskImage>()
                .HasOne(t => t.StaffTask)
                .WithMany(a => a.TaskImages)
                .HasForeignKey(t => t.TaskId);


            // Schedule_Staff Configuration
            modelBuilder.Entity<ScheduleDetail>()
                .HasKey(st => st.Id);
            modelBuilder.Entity<ScheduleDetail>()
                .HasOne(st => st.Account)
                .WithMany(st => st.ScheduleTasks)
                .HasForeignKey(st => st.AccountId)
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

            //Material_Service Configuration
            modelBuilder.Entity<Material_Service>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Material_Service>()
                .HasOne(m => m.Material)
                .WithMany(m => m.Material_Services)
                .HasForeignKey(m => m.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Material_Service>()
                .HasOne(m => m.Service)
                .WithMany(m => m.Material_Services)
                .HasForeignKey(m => m.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
