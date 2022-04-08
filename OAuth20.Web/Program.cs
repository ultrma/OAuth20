using OAuth20.LineClient.Services;
using OAuth20.LineClient.Models;
using OAuth20.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using OAuth20.Web.Models;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions());
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
    options.LoginPath = "/user/login";
    options.AccessDeniedPath = "//user/accessdenied";
    options.LogoutPath = "/user/logout";
});
builder.Services.AddAuthorization(o => o.AddPolicy("AdminsOnly", b => b.RequireClaim(ClaimTypes.Role, "Admin")));

builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("AdminSettings"));
builder.Services.Configure<LineLoginSettings>(builder.Configuration.GetSection("LineLoginSettings"));
builder.Services.Configure<LineNotifySettings>(builder.Configuration.GetSection("LineNotifySettings"));
builder.Services.AddSingleton<LoginUsers>();
builder.Services.AddTransient<ILineNotifyClient, LineNotifyClient>();
builder.Services.AddTransient<ILineLoginClient, LineLoginClient>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
