using Hangfire;
using HangfireBasicAuthenticationFilter;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Host.UseSerilog((Context, Configuration) =>
{
    Configuration.ReadFrom.Configuration(Context.Configuration);
}
);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseHangfireDashboard("/Jobs",new DashboardOptions
{
    Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
    {
        User = builder.Configuration.GetValue<string>("HangfireSettings:UserName"),
        Pass = builder.Configuration.GetValue<string>("HangfireSettings:Password")
    } },
    DashboardTitle= "SurveyBasketJobs "
});
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using var scope = scopeFactory.CreateScope();
var notificationsService = scope.ServiceProvider.GetRequiredService<INotificationsService>();
RecurringJob.AddOrUpdate("SendNewPollNotification", () => notificationsService.SendNewPollNotificationAsync(null),
    Cron.Daily);

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseCors("allowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();
