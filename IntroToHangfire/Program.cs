using Hangfire;
using Hangfire.SqlServer;
using IntroToHangfire;
using IntroToHangfire.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<ApplicationDbContext>(options => 
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddDbContextPool<ApplicationDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped< IPeopleRepository, PeopleRepository > ();
builder.Services.AddTransient<ITimeService, TimeService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));


builder.Services.AddHangfireServer();

////this will override normal 15 seconds of server request to possible 1 second
//builder.Services.AddHangfireServer(options =>
//    options.SchedulePollingInterval = TimeSpan.FromSeconds(5));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ITimeService>("print-time", service => service.PrintNow(), Cron.Daily);

app.Run();
