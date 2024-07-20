using System.Reflection;
using MediatR;
using FluentValidation;
using BuyAndSell.ViewModels;
using BuyAndSell.Validators.UserModelValidator;
using BuyAndSell.Services;
using BuyAndSell.Сommon.Behaviors;
using BuyAndSell.Data;
using BuyAndSell.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer; //it will be need
using Microsoft.EntityFrameworkCore;
using BuyAndSell.Validators;
using BuyAndSell.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdService, AdService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient<IValidator<RegisterViewModel>, RegisterViewModelValidator>();
builder.Services.AddTransient<IValidator<LoginViewModel>, LoginViewModelValidator>();
builder.Services.AddTransient<IValidator<Ad>, AdValidator>();

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders(); // Уберите эту строку, так как она очищает все провайдеры логирования, включая добавленный ранее провайдер для файла
    logging.AddConsole(); // Оставьте только эту строку, если вы хотите логировать в консоль
    logging.AddFile("logs/BuyAndSell.log");
});

#region
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });*/
#endregion
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(policy =>
    policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapDefaultControllerRoute();
});

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

}

app.Run();
