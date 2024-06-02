using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RoadReady.Contexts;
using RoadReady.Interface;
using RoadReady.Models;
using RoadReady.Repositories;
using RoadReady.Services;
using System.Text;
using System.Text.Json.Serialization;

namespace RoadReady
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add services to the container.

            //builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                opts.JsonSerializerOptions.WriteIndented = true;
            });
           
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                    });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"])),
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidIssuer = builder.Configuration["IdentityServer4:Authority"],
                     ValidAudience = builder.Configuration["IdentityServer4:Audience"]
                 };
             });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactPolicy", opts =>
                {
                    opts.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();                    
                });
            });

            #region Context
            //AddDbContext is lifetime but works as singleton
            builder.Services.AddDbContext<CarRentalDbContext>(opts =>
            {
                 opts.UseSqlServer(builder.Configuration.GetConnectionString("RoadReadyConnection"));
            });
            #endregion

            #region Repository Injection
            builder.Services.AddScoped<IRepository<int, Admin>, AdminRepository>();
            builder.Services.AddScoped<IRepository<int, Car>, CarRepository>();
            builder.Services.AddScoped<IRepository<int, CarStore>, CarStoreRepository>();
            builder.Services.AddScoped<IRepository<int, Discount>, DiscountRepository>();
            builder.Services.AddScoped<IRepository<int, Payment>,PaymentRepository>();
            builder.Services.AddScoped<IRepository<int, RentalStore>,RentalStoreRepository>();
            builder.Services.AddScoped<IRepository<int, Reservation>,ReservationRepository>();
            builder.Services.AddScoped<IRepository<int, Review>,ReviewRepository>();
            builder.Services.AddScoped<IRepository<int, User>,UserRepository>();
            builder.Services.AddScoped<IRepository<string, Validation>,ValidationRepository>();

            #endregion

            #region Service Injection

            builder.Services.AddScoped<IAdminService, AdminService>();
           
            builder.Services.AddScoped<ICarAdminService, CarService>();
            builder.Services.AddScoped<ICarUserService, CarService>();
            
            builder.Services.AddScoped<ICarStoreAdminService, CarStoreService>();
            builder.Services.AddScoped<ICarStoreUserService, CarStoreService>();

            builder.Services.AddScoped<IDiscountAdminService, DiscountService>();
            builder.Services.AddScoped<IDiscountUserService, DiscountService>();
           
            builder.Services.AddScoped<IPaymentAdminService, PaymentService>();
            builder.Services.AddScoped<IPaymentUserService, PaymentService>();
            
            builder.Services.AddScoped<IRentalStoreAdminService, RentalStoreService>();
            builder.Services.AddScoped<IRentalStoreUserService, RentalStoreService>();

            builder.Services.AddScoped<IReservationAdminService, ReservationService>();
            builder.Services.AddScoped<IReservationUserService, ReservationService>();
           
            builder.Services.AddScoped<IReviewAdminService, ReviewService>();
            builder.Services.AddScoped<IReviewUserService, ReviewService>();
           
            builder.Services.AddScoped<IUserUserService, UserService>();
            builder.Services.AddScoped<IUserAdminService, UserService>();

            builder.Services.AddScoped<IValidationService, ValidationService>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("ReactPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
                          
            app.MapControllers();

            app.Run();
        }
    }
}
