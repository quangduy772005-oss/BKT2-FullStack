using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PCM.Application;
using PCM.Core.Entities;
using PCM.Infrastructure;
using PCM.Infrastructure.Persistence;
using PCM.Infrastructure.Hubs;
using Hangfire;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Configuration
// =====================
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
var jwtAudience = builder.Configuration["Jwt:Audience"]!;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
var redisConnectionString = builder.Configuration.GetConnectionString("Redis"); // Optional

// =====================
// Add Services (Dependency Injection)
// =====================

// Application Layer Services
builder.Services.AddApplicationServices();

// Infrastructure Layer Services (Redis is optional)
builder.Services.AddInfrastructureServices(connectionString, redisConnectionString);

// Identity
builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Controllers with JSON serialization fix
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVue", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        
        // SignalR support
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

// Authorization
builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PCM Pro API",
        Version = "v1.0",
        Description = "Pickleball Club Management - Professional Edition"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token JWT: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Hangfire (Background Jobs)
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

// =====================
// Build App
// =====================
var app = builder.Build();

// =====================
// Database Initialization
// =====================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Apply migrations
    await dbContext.Database.MigrateAsync();

    // Seed data
    await SeedRolesAndAdminUser(roleManager, userManager);
}

// =====================
// Middleware Pipeline
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PCM Pro API v1");
        c.RoutePrefix = string.Empty;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowVue");

app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.MapControllers();

// SignalR
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();

// =====================
// Seed Roles and Admin User
// =====================
async Task SeedRolesAndAdminUser(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
{
    try
    {
        Console.WriteLine("--- Seeding Roles and Admin User ---");
        // Create roles
        string[] roles = { "Admin", "Treasurer", "Member", "Referee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
                if (roleResult.Succeeded) Console.WriteLine($"Created role: {role}");
            }
        }

        // Create admin user
        var adminEmail = "admin@pcm.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            Console.WriteLine("Creating admin user...");
            adminUser = new AppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "Admin",
                LastName = "PCM",
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine("Admin user created successfully.");
                
                // Create Member record for admin
                var member = new Member
                {
                    UserId = adminUser.Id,
                    FullName = "Admin PCM",
                    Email = adminEmail,
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };
                
                // We need the DbContext to save Member directly if navigations are tricky
                // But let's try the navigation first
                adminUser.Member = member;
                var updateResult = await userManager.UpdateAsync(adminUser);
                if (updateResult.Succeeded) Console.WriteLine("Admin Member record linked.");
                else Console.WriteLine("Failed to link Admin Member record: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }
            else
            {
                Console.WriteLine("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine("Admin user already exists.");
            // Ensure Member record exists for admin
            // Use find instead of Include to be safer
            var existingMember = await userManager.Users
                .Where(u => u.Id == adminUser.Id)
                .Select(u => u.Member)
                .FirstOrDefaultAsync();

            if (existingMember == null)
            {
                Console.WriteLine("Linking missing Member record for existing Admin...");
                var member = new Member
                {
                    UserId = adminUser.Id,
                    FullName = "Admin PCM",
                    Email = adminEmail,
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };
                adminUser.Member = member;
                await userManager.UpdateAsync(adminUser);
            }

            // Ensure EmailConfirmed and IsActive
            if (!adminUser.EmailConfirmed || !adminUser.IsActive)
            {
                adminUser.EmailConfirmed = true;
                adminUser.IsActive = true;
                await userManager.UpdateAsync(adminUser);
            }
        }

        // Create teacher account for testing
        var teacherEmail = "teacher@pcm.local";
        var teacherUser = await userManager.FindByEmailAsync(teacherEmail);
        if (teacherUser == null)
        {
            Console.WriteLine("Creating teacher account...");
            teacherUser = new AppUser
            {
                UserName = teacherEmail,
                Email = teacherEmail,
                FirstName = "Giao Vien",
                LastName = "Cham Bai",
                EmailConfirmed = true,
                IsActive = true,
                PhoneNumber = "0123456789"
            };

            var result = await userManager.CreateAsync(teacherUser, "Teacher@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(teacherUser, "Member");
                Console.WriteLine("Teacher user created successfully.");
                
                // Create Member record for teacher
                var teacherMember = new Member
                {
                    UserId = teacherUser.Id,
                    FullName = "Giao Vien Cham Bai",
                    Email = teacherEmail,
                    PhoneNumber = "0123456789",
                    JoinDate = DateTime.UtcNow,
                    IsActive = true
                };
                
                teacherUser.Member = teacherMember;
                var updateResult = await userManager.UpdateAsync(teacherUser);
                if (updateResult.Succeeded) Console.WriteLine("Teacher Member record linked.");
                else Console.WriteLine("Failed to link Teacher Member record: " + string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            }
            else
            {
                Console.WriteLine("Failed to create teacher user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            Console.WriteLine("Teacher user already exists.");
        }

        Console.WriteLine("--- Seeding Completed ---");
    }
    catch (Exception ex)
    {
        Console.WriteLine("CRITICAL ERROR DURING SEEDING: " + ex.Message);
        Console.WriteLine(ex.StackTrace);
    }
}
