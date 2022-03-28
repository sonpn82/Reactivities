using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities;
using Application.Core;
using Application.Interfaces;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, 
        IConfiguration config)
        {
             services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAPIv5", Version = "v1" });
            });
            services.AddDbContext<DataContext>(opt =>  // class DataContext in Persistence.cs
            {
                opt.UseSqlite(config.GetConnectionString("DefaultConnection")); // DefaultConnection input from appsettings.Development.json
            });
            // this service to allow client site get data from server API - CORS - cross origin resources s
            services.AddCors(opt => 
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {  // Allow localhost:3000 to access API data
                    policy.
                        AllowAnyMethod().
                        AllowAnyHeader().
                        AllowCredentials().  // for SignalR - in creating comments 
                        WithOrigins("http://localhost:3000");
                
                });
            });
            // Add mediator service - tell where mediator are / where handler is
            services.AddMediatR(typeof(List.Handler).Assembly);
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddScoped<IUserAccessor, UserAccessor>();  // to get the username of current user from anywhere in our app
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();  // add service to allow access photo in cloudinary            
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary")); // Cloudinary = section name in Appsettings.json
            services.AddSignalR();  // for adding comment and send comment to all activity participants in real time
            
            return services;
        }
    }
}