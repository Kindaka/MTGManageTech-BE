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
    options.AddPolicy("RequireAllRoles", policy => policy.RequireClaim(ClaimTypes.Role, "1", "3", "4", "5"));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireClaim(ClaimTypes.Role, "5"));
    options.AddPolicy("RequireManagerOrStaffRole", policy => policy.RequireClaim(ClaimTypes.Role, "3", "5"));
    options.AddPolicy("RequireManagerOrStaffOrCustomerRole", policy => policy.RequireClaim(ClaimTypes.Role, "3", "4", "5"));
});

// Đăng ký AutoMapper với cấu hình ánh xạ
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Đăng ký các dịch vụ của bạn
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IMartyrGraveService, MartyrGraveService>();
builder.Services.AddScoped<IMartyrGraveInformationService, MartyrGraveInformationService>();
builder.Services.AddScoped<IServiceCategory_Service, ServiceCategory_Service>();
builder.Services.AddScoped<IGraveService_Service, GraveService_Service>();
builder.Services.AddScoped<ICartService, CartItemsService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
