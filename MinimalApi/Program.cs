using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup => setup.SwaggerDoc("v1", new OpenApiInfo()
{
    Description = "Product API using .Net 6 minimal web API, SQL Server, Docker Dev Environments",
    Title = "Product API",
    Version = "v1"
})
);
builder.Services.AddValidatorsFromAssemblyContaining<Product>(lifetime: ServiceLifetime.Scoped);
builder.Services.AddDbContext<ProductContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection")));
builder.Services.AddLogging();
var app = builder.Build();

await EnsureDb(app.Services, app.Logger);

app.UseSwagger();

app.MapGet("/", () => "Hello World!");

app.MapGet("/product/{pageNumber}", async (int pageNumber, ProductContext dbcontext) =>
{
    var pageSize = app.Configuration.GetValue<int>("AppSettings:PageSize");
    var products = await dbcontext.Products
        .Skip(pageNumber * pageSize)
        .Take(pageSize)
        .ToListAsync();
    app.Logger.LogInformation($"/product Count: {products.Count}");
    return Results.Ok(products);
});

app.MapGet("/product/find/{id}", async (int id, ProductContext dbcontext) =>
{
    var result = await dbcontext.Products.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound(id);
    app.Logger.LogInformation($"/product/{id}:");
    return result;
});


app.MapGet("/product/count", async (ProductContext dbcontext) =>
{
    var count = await dbcontext.Products.CountAsync();
    app.Logger.LogInformation($"/product/count: {count}");
    return Results.Ok(count);
});

app.MapGet("/product/search/{searchTerm}/{pageNumber}", async (string searchTerm, int pageNumber, ProductContext dbcontext) =>
{
    var pageSize = app.Configuration.GetValue<int>("AppSettings:PageSize");
    var products = await dbcontext.Products
        .Where(x => x.Name.Contains(searchTerm))
        .Skip(pageNumber * pageSize)
        .Take(pageSize)
        .ToListAsync();
    app.Logger.LogInformation($"/product/search Count: {products?.Count}");
    return Results.Ok(products);
});

app.MapPost("/product", async (Product product, ProductContext dbcontext, IValidator<Product> validator) =>
{
    var validationResult = validator.Validate(product);
    if (!validationResult.IsValid)
    {
        return Results.ValidationProblem(validationResult.ToDictionary());
    }

    dbcontext.Products.Add(product);
    await dbcontext.SaveChangesAsync();
    app.Logger.LogInformation($"/product ID Created: {product?.Id}");
    return Results.Created($"/product/{product.Id}", product);
});

app.UseSwaggerUI();

app.Run();

async Task EnsureDb(IServiceProvider services, ILogger logger)
{
    using (var db = services.CreateScope().ServiceProvider.GetRequiredService<ProductContext>())
    {
        logger.LogInformation("Ensuring database exists and is up to date at connection string '{connectionString}'");
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }
}

public class ProductContext : DbContext
{
    public ProductContext(DbContextOptions<ProductContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
}

public class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; }

    [MaxLength(1024)]
    public string Description { get; set; }

    public class Validator : AbstractValidator<Product>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotNull().WithMessage("Name must not null");
            RuleFor(x => x.Name).MinimumLength(1).MaximumLength(128).WithMessage("Name must contain between 1 and 128 characters");
            RuleFor(x => x.Description).MaximumLength(1024).WithMessage("Description cannot contain more than 1024 characters");
        }
    }
}

public static class ValidationExtensions
{
    public static IDictionary<string, string[]> ToDictionary(this FluentValidation.Results.ValidationResult validationResult)
    {
        return validationResult.Errors
                       .GroupBy(x => x.PropertyName)
                       .ToDictionary(
                           g => g.Key,
                           g => g.Select(x => x.ErrorMessage).ToArray()
                       );
    }
}

// Make Program class visible to test projects
public partial class Program { }