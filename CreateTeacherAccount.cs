using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PCM.Core.Entities;
using PCM.Infrastructure.Persistence;

// This script creates a teacher account for testing
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// Create teacher account
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var teacherEmail = "teacher@pcm.local";
    var existingUser = await userManager.FindByEmailAsync(teacherEmail);

    if (existingUser == null)
    {
        Console.WriteLine("Creating teacher account...");
        
        var teacher = new AppUser
        {
            UserName = teacherEmail,
            Email = teacherEmail,
            FirstName = "Giao Vien",
            LastName = "Cham Bai",
            EmailConfirmed = true,
            IsActive = true,
            PhoneNumber = "0123456789"
        };

        var result = await userManager.CreateAsync(teacher, "Teacher@123");
        
        if (result.Succeeded)
        {
            // Create Member record
            var member = new Member
            {
                UserId = teacher.Id,
                FullName = "Giao Vien Cham Bai",
                Email = teacherEmail,
                PhoneNumber = "0123456789",
                JoinDate = DateTime.UtcNow,
                IsActive = true
            };
            
            context.Members.Add(member);
            await context.SaveChangesAsync();

            // Add Member role
            await userManager.AddToRoleAsync(teacher, "Member");

            Console.WriteLine("✅ Teacher account created successfully!");
            Console.WriteLine($"📧 Email: {teacherEmail}");
            Console.WriteLine("🔑 Password: Teacher@123");
        }
        else
        {
            Console.WriteLine("❌ Failed to create teacher account:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"   - {error.Description}");
            }
        }
    }
    else
    {
        Console.WriteLine("⚠️ Teacher account already exists!");
        Console.WriteLine($"📧 Email: {teacherEmail}");
        Console.WriteLine("🔑 Password: Teacher@123");
    }
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
