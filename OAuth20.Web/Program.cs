using OAuth20.LineClient.Services;
using OAuth20.LineClient.Models;
using OAuth20.Web.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions());

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var config = builder.Configuration;
builder.Services.Configure<LineLoginSettings>(config.GetSection("LineLoginSettings"));
builder.Services.Configure<LineNotifySettings>(config.GetSection("LineNotifySettings"));

builder.Services.AddSingleton<LineNotifySubscriptions>();
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

app.UseAuthorization();

app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
