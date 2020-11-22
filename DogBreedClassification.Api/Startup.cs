using DogBreedClassification.Api.EF;
using DogBreedClassification.Api.Models;
using DogBreedClassification.Api.Services;
using DogBreedClassification.Api.Settings;
using DogBreedClassification.Shared.DataModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.ML;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace DogBreedClassification.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddPredictionEnginePool<InMemoryImageData, ImagePrediction>()
                        .FromFile(Configuration["MLModel:MLModelFilePath"]);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", p => p.RequireRole("admin"));
                options.AddPolicy("User", p => p.RequireAuthenticatedUser());
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg => {
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SecretKey"])),
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddDbContext<DogBreedClassificationContext>(options =>
            {
                options.UseSqlServer(Configuration["ConnectionStrings:DogBreedClassificationDb"]);
            });

            services.AddSwaggerGen();

            services.AddTransient<JwtSettings>();
            services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();

            services.AddSingleton<IJwtHandler, JwtHandler>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDogClassificationService, DogClassificationService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
