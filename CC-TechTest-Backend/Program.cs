using CC_TechTest_Backend.Configuration;
using CC_TechTest_Backend.Data;
using CC_TechTest_Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

builder.Configuration.AddJsonFile("Configuration/config.json", optional: false, reloadOnChange: true);
builder.Services.Configure<Config>(builder.Configuration.GetSection("Config"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy => 
        policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

if (builder.Configuration.GetSection("Config").Get<Config>().useInMemoryStorage)
    builder.Services.AddSingleton<IDataStore, InMemoryStorage>();
else
{
    builder.Services.AddDbContext<MeterDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
    builder.Services.AddTransient<IDataStore, QueryContext>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
