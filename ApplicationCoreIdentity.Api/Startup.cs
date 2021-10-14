using ApplicationCoreIdentity.Models;
using ApplicationCoreIdentity.Services.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace ApplicationCoreIdentity.Api
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
            var connection = Configuration.GetConnectionString("ApplicationString");
            var connectionAuthentication = Configuration.GetConnectionString("AuthenticationString");

            var authenticationIssuer = Configuration["Tokens:Issuer"];
            var authenticationAudience = Configuration["Tokens:Audience"];
            var authenticationKey = Configuration["Tokens:Key"];

            services.AddControllers();

            services.AddSwaggerGen();

            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(
                    connection,
                    migrations => migrations.MigrationsAssembly("ApplicationCoreIdentity.Models")
                )
            );

            services.AddDbContext<AuthenticationDbContext>(
                options => options.UseSqlServer(
                    connectionAuthentication
                )
            );

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AuthenticationDbContext>();

            services.Configure<IdentityOptions>(
                options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                }
            );

            services.AddAuthentication().AddJwtBearer(
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = authenticationIssuer,
                        ValidAudience = authenticationAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationKey)),
                        RequireExpirationTime = false,
                        //ValidateIssuer = false,
                        //ValidateAudience = false
                    };
                }
            );

            services.AddCors(
                options =>
                {
                    options.AddPolicy("DevelopmentApiPolicy", cors =>
                    {
                        cors.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
                    });
                }
            );

            services.AddSingleton<IAuthenticationConfiguration>(x =>
                new AuthenticationConfiguration
                {
                    Issuer = authenticationIssuer,
                    Audience = authenticationAudience,
                    Key = authenticationKey
                }
            );

            services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());
            services.AddTransient<IAuthenticationService, AuthenticationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApplicationCoreIdentity Api");
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("DevelopmentApiPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
