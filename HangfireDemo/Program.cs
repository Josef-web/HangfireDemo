using Hangfire;
using Hangfire.Common;
using HangfireBasicAuthenticationFilter;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire((sp,x) =>
{
    var hangfireConnectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DbConnection");
    x.UseSqlServerStorage(hangfireConnectionString);
});
builder.Services.AddHangfireServer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHangfireDashboard("/test/job-dashboard", new DashboardOptions
{
    DashboardTitle = "Hangfire Job Demo Application",
    DisplayStorageConnectionString = false,
    Authorization = new []
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin",
            Pass = "admin123"
        }
        
    }
});

app.MapControllers();

app.Run();