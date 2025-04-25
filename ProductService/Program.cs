using Microsoft.EntityFrameworkCore;
using ProductService.API.Mapping;
using ProductService.Data;
using ProductService.Data.Application;
using ProductService.Data.Repository;

namespace ProductService;

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

        builder.Services.AddDbContext<ProductServiceDbContext>(options =>
        {
            options.UseMySQL(connectionString);
        });
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IProductService, Data.Application.ProductService>();
        builder.Services.AddAutoMapper(typeof(ProductMapping));
        builder.Services.AddAutoMapper(typeof(CategoryMapping));

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