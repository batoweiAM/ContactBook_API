using CloudinaryDotNet;
using Contact_book_Application.API.Repository;
using ContactBook.Data;
using ContactBook.Data.DataInitializer;
using ContactBook.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContextPool<ContactBookContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Repository registration
builder.Services.AddScoped<IRepository, Repository>();
                
//Identity registration
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ContactBookContext>();

//Cloudinary registration
builder.Services.AddSingleton(new Cloudinary(new Account(
    "dt2irsses",
    "668474174267951",
    "-t5aKMdNkkmKozsD1vvJdcFnMPg"
    )));

//Authentication registration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:JWTSigningkey").Value)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

//Adding Documentation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contact Book", Version = "v1.0.0" });
    var securitySchema = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securitySchema);
    var securityRequirement = new OpenApiSecurityRequirement
    {
    { securitySchema, new[] { "Bearer" } }
    };
    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Get the service scope and obtain the necessary services
using var scope = app.Services.CreateScope();
var serviceProvider = scope.ServiceProvider;
var context = serviceProvider.GetRequiredService<ContactBookContext>();
var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

UserAndRoleDataInitializer.SeedData(context, userManager, roleManager).Wait();

app.MapControllers();

app.Run();
