using Microsoft.EntityFrameworkCore;
using OrderService.API.Mapping;
using OrderService.Application;
using OrderService.Data;
using OrderService.Data.Repository;

namespace OrderService;

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

        builder.Services.AddDbContext<OrderServiceDbContext>(options =>
        {
            options.UseMySQL(connectionString);
        });
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IOrderService, OrderService.Application.OrderService>();
        builder.Services.AddAutoMapper(typeof(OrderMapping));

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