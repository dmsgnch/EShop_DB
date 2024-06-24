using EShop_DB.Components;
using EShop_DB.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("WebApiDatabase"/*"sqlConnection"*/) ?? 
                    throw new Exception("There are no any connection string with the same name \"DefaultConnection\"");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));

// string connectionTest = builder.Configuration.GetConnectionString("TestConnectionString") ?? 
//                     throw new Exception("There are no any connection string with the same name \"TestConnectionString\"");
//
// builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionTest));

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

AppDbInitializer.Seed(app);

app.Run();