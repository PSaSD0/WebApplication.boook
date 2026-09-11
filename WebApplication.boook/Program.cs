using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApplication.boook.Data;
using WebApplication.boook.Models;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Add database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// AUTO-CREATE DATABASE (без миграций)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Эта строка создаст базу и таблицы если их нет
    db.Database.EnsureCreated();
    
    // Добавляем тестовые данные
    if (!db.GuestBookEntries.Any())
    {
        db.GuestBookEntries.AddRange(
            new GuestBookEntry { Author = "Админ", Text = "Добро пожаловать!", CreatedAt = DateTime.UtcNow },
            new GuestBookEntry { Author = "Юзер", Text = "Тест работает!", CreatedAt = DateTime.UtcNow.AddHours(-1) }
        );
        db.SaveChanges();
    }
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=GuestBook}/{action=Index}/{id?}");

app.Run();