using AddressBookService.API.Mapping;
using AddressBookService.Application;
using AddressBookService.Data;
using AddressBookService.Data.Repository;
using Microsoft.EntityFrameworkCore;

namespace AddressBookService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        string connectionString;
        if (builder.Environment.IsDevelopment())
        {
            connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? string.Empty;
        }
        else
        {
            connectionString = builder.Configuration["ConnectionStrings:MySqlConnection"] ?? string.Empty;
        }

        builder.Services.AddDbContext<AddressBookServiceDbContext>(options =>
        {
            options.UseMySQL(connectionString);
        });
        builder.Services.AddScoped<IAddressBookRepository, AddressBookRepository>();
        builder.Services.AddScoped<IAddressBookService, Application.AddressBookService>();
        builder.Services.AddAutoMapper(typeof(AddressMapping));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        //{
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
        //}

        app.MapControllers();
        app.UseHttpsRedirection();
        app.Run();
    }
}