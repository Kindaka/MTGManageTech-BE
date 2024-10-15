﻿// <auto-generated />
using System;
using MartyrGraveManagement_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MartyrGraveManagement_DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241015093821_RemoveStaffJobTable")]
    partial class RemoveStaffJobTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AccountId"));

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("AvatarPath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("CustomerCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("AccountId");

                    b.HasIndex("RoleId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Area", b =>
                {
                    b.Property<int>("AreaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AreaId"));

                    b.Property<string>("AreaName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("AreaId");

                    b.ToTable("Areas");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.CartItem", b =>
                {
                    b.Property<int>("CartId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CartId"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<int>("MartyrId")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("CartId");

                    b.HasIndex("AccountId");

                    b.HasIndex("MartyrId");

                    b.HasIndex("ServiceId");

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Feedback", b =>
                {
                    b.Property<int>("FeedbackId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FeedbackId"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("FeedbackId");

                    b.HasIndex("AccountId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Feedbacks");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.FeedbackResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("FeedbackId")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("FeedbackId")
                        .IsUnique();

                    b.ToTable("FeedbackResponses");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.GraveImage", b =>
                {
                    b.Property<int>("ImageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ImageId"));

                    b.Property<int>("MartyrId")
                        .HasColumnType("int");

                    b.Property<string>("UrlPath")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ImageId");

                    b.HasIndex("MartyrId");

                    b.ToTable("GraveImage");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.MartyrGrave", b =>
                {
                    b.Property<int>("MartyrId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MartyrId"));

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<int>("AreaNumber")
                        .HasColumnType("int");

                    b.Property<string>("CustomerCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MartyrCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MartyrNumber")
                        .HasColumnType("int");

                    b.Property<int>("RowNumber")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("MartyrId");

                    b.HasIndex("AreaId");

                    b.ToTable("MartyrGraves");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.MartyrGraveInformation", b =>
                {
                    b.Property<int>("InformationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InformationId"));

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateOfSacrifice")
                        .HasColumnType("datetime2");

                    b.Property<string>("HomeTown")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("MartyrId")
                        .HasColumnType("int");

                    b.Property<string>("Medal")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NickName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("InformationId");

                    b.HasIndex("MartyrId");

                    b.ToTable("MartyrGraveInformations");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Material", b =>
                {
                    b.Property<int>("MaterialId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MaterialId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MaterialName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.HasKey("MaterialId");

                    b.HasIndex("ServiceId");

                    b.ToTable("Material");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrderId"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ResponseContent")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<decimal>("TotalPrice")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("OrderId");

                    b.HasIndex("AccountId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.OrderDetail", b =>
                {
                    b.Property<int>("DetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DetailId"));

                    b.Property<int>("MartyrId")
                        .HasColumnType("int");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<double>("OrderPrice")
                        .HasColumnType("float");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("ServiceId")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("DetailId");

                    b.HasIndex("MartyrId");

                    b.HasIndex("OrderId");

                    b.HasIndex("ServiceId");

                    b.ToTable("OrderDetails");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Payment", b =>
                {
                    b.Property<int>("PaymentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PaymentId"));

                    b.Property<string>("BankCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankTransactionNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CardType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<DateTime>("PayDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("PaymentAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PaymentInfo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionNo")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TransactionStatus")
                        .HasColumnType("int");

                    b.HasKey("PaymentId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Service", b =>
                {
                    b.Property<int>("ServiceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ServiceId"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<string>("ServiceName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("ServiceId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.ServiceCategory", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("UrlImageCategory")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryId");

                    b.ToTable("ServiceCategories");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.StaffTask", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TaskId"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImagePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameOfWork")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OrderId")
                        .HasColumnType("int");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TypeOfWork")
                        .HasColumnType("int");

                    b.Property<string>("UrlImage")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TaskId");

                    b.HasIndex("AccountId");

                    b.HasIndex("OrderId")
                        .IsUnique();

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.WeeklyReportGrave", b =>
                {
                    b.Property<int>("WeeklyReportId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WeeklyReportId"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DisciplinePoint")
                        .HasColumnType("int");

                    b.Property<int>("MartyrId")
                        .HasColumnType("int");

                    b.Property<int>("QualityOfFlowerPoint")
                        .HasColumnType("int");

                    b.Property<int>("QualityOfTotalGravePoint")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.HasKey("WeeklyReportId");

                    b.HasIndex("MartyrId");

                    b.ToTable("WeeklyReportGraves");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.WorkPerformance", b =>
                {
                    b.Property<int>("WorkId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WorkId"));

                    b.Property<int>("AccountId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InteractionPoint")
                        .HasColumnType("int");

                    b.Property<int>("QualityMaintenancePoint")
                        .HasColumnType("int");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<int>("TimeCompletePoint")
                        .HasColumnType("int");

                    b.Property<DateTime>("UploadTime")
                        .HasColumnType("datetime2");

                    b.HasKey("WorkId");

                    b.HasIndex("AccountId");

                    b.ToTable("WorkPerformances");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Account", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.CartItem", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Account", "Account")
                        .WithMany("CartItems")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.MartyrGrave", "MartyrGrave")
                        .WithMany("CartItems")
                        .HasForeignKey("MartyrId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.Service", "Service")
                        .WithMany("CartItems")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("MartyrGrave");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Feedback", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Account", "Account")
                        .WithMany("Feedbacks")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.Order", "Order")
                        .WithOne("Feedback")
                        .HasForeignKey("MartyrGraveManagement_DAL.Entities.Feedback", "OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.FeedbackResponse", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Account", "Account")
                        .WithMany("FeedbackResponses")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.Feedback", "Feedback")
                        .WithOne("Response")
                        .HasForeignKey("MartyrGraveManagement_DAL.Entities.FeedbackResponse", "FeedbackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Feedback");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.GraveImage", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.MartyrGrave", "MartyrGrave")
                        .WithMany("GraveImages")
                        .HasForeignKey("MartyrId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MartyrGrave");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.MartyrGrave", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Area", "Area")
                        .WithMany("MartyrGraves")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.MartyrGraveInformation", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.MartyrGrave", "MartyrGrave")
                        .WithMany("MartyrGraveInformations")
                        .HasForeignKey("MartyrId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MartyrGrave");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Material", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Service", "Service")
                        .WithMany("Materials")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Service");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Order", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Account", "Account")
                        .WithMany("Orders")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.OrderDetail", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.MartyrGrave", "MartyrGrave")
                        .WithMany("OrderDetails")
                        .HasForeignKey("MartyrId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.Service", "Service")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ServiceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MartyrGrave");

                    b.Navigation("Order");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Payment", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Order", "Order")
                        .WithOne("Payment")
                        .HasForeignKey("MartyrGraveManagement_DAL.Entities.Payment", "OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Order");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Service", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.ServiceCategory", "ServiceCategory")
                        .WithMany("Services")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ServiceCategory");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.StaffTask", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Account", "Account")
                        .WithMany("Tasks")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MartyrGraveManagement_DAL.Entities.Order", "Order")
                        .WithOne("Task")
                        .HasForeignKey("MartyrGraveManagement_DAL.Entities.StaffTask", "OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.WeeklyReportGrave", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.MartyrGrave", "MartyrGrave")
                        .WithMany("WeeklyReportGraves")
                        .HasForeignKey("MartyrId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MartyrGrave");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.WorkPerformance", b =>
                {
                    b.HasOne("MartyrGraveManagement_DAL.Entities.Account", "Account")
                        .WithMany("WorkPerformances")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Account", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("FeedbackResponses");

                    b.Navigation("Feedbacks");

                    b.Navigation("Orders");

                    b.Navigation("Tasks");

                    b.Navigation("WorkPerformances");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Area", b =>
                {
                    b.Navigation("MartyrGraves");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Feedback", b =>
                {
                    b.Navigation("Response");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.MartyrGrave", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("GraveImages");

                    b.Navigation("MartyrGraveInformations");

                    b.Navigation("OrderDetails");

                    b.Navigation("WeeklyReportGraves");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Order", b =>
                {
                    b.Navigation("Feedback");

                    b.Navigation("OrderDetails");

                    b.Navigation("Payment");

                    b.Navigation("Task");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Role", b =>
                {
                    b.Navigation("Accounts");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.Service", b =>
                {
                    b.Navigation("CartItems");

                    b.Navigation("Materials");

                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("MartyrGraveManagement_DAL.Entities.ServiceCategory", b =>
                {
                    b.Navigation("Services");
                });
#pragma warning restore 612, 618
        }
    }
}
