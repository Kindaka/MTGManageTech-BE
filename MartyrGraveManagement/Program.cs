using MartyrGraveManagement_BAL.Services.Implements;
using MartyrGraveManagement_BAL.Services.Interfaces;
using MartyrGraveManagement_DAL.Entities;
using MartyrGraveManagement_DAL.UnitOfWorks.Implements;
using MartyrGraveManagement_DAL.UnitOfWorks.Interfaces;
using MartyrGraveManagement_BAL.MappingProfiles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.SqlServer;
using MartyrGraveManagement.BackgroundServices.Implements;
using MartyrGraveManagement.BackgroundServices.Interfaces;
using MartyrGraveManagement_BAL.BackgroundServices.Interfaces;
using MartyrGraveManagement_BAL.BackgroundServices.Implements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Cấu hình Swagger để đọc file XML và hiển thị các mô tả API
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// JWT Configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Set policy permission for roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "1"));
    options.AddPolicy("RequireStaffRole", policy => policy.RequireClaim(ClaimTypes.Role, "3"));
    options.AddPolicy("RequireCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "4"));
    options.AddPolicy("RequireAdminOrStaffRole", policy => policy.RequireClaim(ClaimTypes.Role, "1", "3"));
    options.AddPolicy("RequireAdminOrCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "1", "4"));
    options.AddPolicy("RequireStaffOrCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "3", "4"));
    options.AddPolicy("RequireAllRoles", policy => policy.RequireClaim(ClaimTypes.Role, "1", "3", "4", "2"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireClaim(ClaimTypes.Role, "2"));
    options.AddPolicy("RequireManagerOrStaffRole", policy => policy.RequireClaim(ClaimTypes.Role, "3", "2"));
    options.AddPolicy("RequireManagerOrStaffOrCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "3", "4", "2"));
    options.AddPolicy("RequireManagerOrStaffOrAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "1", "2", "3"));
    options.AddPolicy("RequireManagerOrAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "1", "2"));
});

// Cấu hình Hangfire để sử dụng SQL Server
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseDefaultTypeSerializer()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

// Add Hangfire server
builder.Services.AddHangfireServer();

// Đăng ký AutoMapper với cấu hình ánh xạ
builder.Services.AddAutoMapper(typeof(MappingProfile));

//Config CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Add Memory Cache
builder.Services.AddMemoryCache();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);


// Đăng ký TaskBackgroundService
builder.Services.AddScoped<ITaskBackgroundService, TaskBackgroundService>();
builder.Services.AddScoped<IOrderBackgroundService, OrderBackgroundService>();
builder.Services.AddScoped<IHolidayEventBackgroundService, HolidayEventBackgroundService>();
//builder.Services.AddScoped<IAttendanceBackgroundService, AttendanceBackgroundService>();
builder.Services.AddScoped<IRecurringTaskService, RecurringTaskService>();


// Đăng ký các dịch vụ của bạn
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IMartyrGraveService, MartyrGraveService>();
builder.Services.AddScoped<IMartyrGraveInformationService, MartyrGraveInformationService>();
builder.Services.AddScoped<IServiceCategory_Service, ServiceCategory_Service>();
builder.Services.AddScoped<IService_Service, Service_Service>();
builder.Services.AddScoped<ICartService, CartItemsService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<ISendEmailService, SendEmailService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IAuthorizeService, AuthorizeService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IGraveService_Service, GraveService_Service>();
builder.Services.AddScoped<IHistoricalEventService, HistoricalEventService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IScheduleDetailService, ScheduleDetailService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ICommentIconService, CommentIconService>();
builder.Services.AddScoped<ICommentReportService, CommentReportService>();
builder.Services.AddScoped<IHolidayEventService, HolidayEventService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ISmsService, TwillioService>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IBlogCategoryService, BlogCategoryService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IServiceSchedule_Service, ServiceSchedule_Service>();
builder.Services.AddScoped<IAssignmentTaskService, AssignmentTaskService>();
builder.Services.AddScoped<IAssignmentTaskFeedbackService, AssignmentTaskFeedbackService>();

// Đăng ký ML
builder.Services.AddScoped<ITrendingRecommendationService, TrendingRecommendationService>();





var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.Map("/", () => "server online");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");


// Schedule the recurring job (Hangfire)
RecurringJob.AddOrUpdate<ITaskBackgroundService>(
    "check-expired-tasks",
    service => service.CheckExpiredTasks(),
    Cron.Hourly());

RecurringJob.AddOrUpdate<IOrderBackgroundService>(
    "check-expired-orders-payment",
    service => service.CheckExpiredOrderPayment(),
    Cron.Hourly());

RecurringJob.AddOrUpdate<IHolidayEventBackgroundService>(
    "check-and-send-holiday-event-notifications",
    service => service.UpdateNotificationAccountsForUpcomingDay(),
    Cron.Hourly());

//RecurringJob.AddOrUpdate<IAttendanceBackgroundService>(
//    "mark-absent-attendance",
//    service => service.MarkAbsentAttendanceAsync(),
//    Cron.Minutely
//);

RecurringJob.AddOrUpdate<IRecurringTaskService>(
    "CreateRecurringTasks",
    service => service.CreateRecurringTasksAsync(),
    Cron.Hourly());



app.MapControllers();
app.Run();
