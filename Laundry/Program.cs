using Laundry.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// 1. Database Connect kia ham nai jh kai ham nai appsetting json mein kya hai 
var connectionString = builder.Configuration.GetConnectionString("LaundryConnection") + ";TrustServerCertificate=True";

builder.Services.AddDbContext<LaundryContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Session add kiay hai kai banda login karai toh uska session start hojai or phir jab session expire hoga toh woh sign out hojai ga 
builder.Services.AddSession();
builder.Services.AddControllersWithViews();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
