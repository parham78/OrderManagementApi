using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<OrderManagementDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<OrderManagementDbContext>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    await IdentitySeeder.SeedRolesAsync(roleManager);
}

app.UseExceptionHandler();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();