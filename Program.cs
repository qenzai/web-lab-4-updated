using Microsoft.EntityFrameworkCore;
using MyWebsiteBackend.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5500") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MyWebsiteBackend", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseCors("AllowLocalhost");

app.UseAuthorization();

app.MapControllers();

try
{
    DbSeeder.Seed(app);
}
catch (Exception ex)
{
    Console.WriteLine("?? Seeder error: " + ex.Message);
    throw;
}

app.Run();
