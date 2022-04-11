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
            // the below code is used before deploy to heroku
            // services.AddDbContext<DataContext>(opt =>  // class DataContext in Persistence.cs
            // {
            //     //opt.UseSqlite(config.GetConnectionString("DefaultConnection")); // DefaultConnection input from appsettings.Development.json
            //     // sqlLite in appsettings.dev.json use this connection string: "Data source=reactivities.db"
            //     // change from sqlite to PostgreSql
            //     // Postgresql use: Server=localhost; Port=5432; User Id=admin; Password=secret; Database=reactivities
            //     // same setting when install PostgreSql in Docker
            //     // delete the migration folder (contain sqlite database initialization)
            //     // create new migration with command:  dotnet ef migrations add PGInitial -p Persistence -s API
            //     opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));  
            // });
            // this service to allow client site get data from server API - CORS - cross origin resources s
            // this code is for deployment in heroku
            services.AddDbContext<DataContext>(options =>
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                string connStr;

                // Depending on if in development or production, use either Heroku-provided
                // connection string, or development connection string from env var.
                if (env == "Development")
                {
                    // Use connection string from file.
                    connStr = config.GetConnectionString("DefaultConnection");
                }
                else
                {
                    // Use connection string provided at runtime by Heroku.
                    var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

                    // Parse connection URL to connection string for Npgsql
                    connUrl = connUrl.Replace("postgres://", string.Empty);
                    var pgUserPass = connUrl.Split("@")[0];
                    var pgHostPortDb = connUrl.Split("@")[1];
                    var pgHostPort = pgHostPortDb.Split("/")[0];
                    var pgDb = pgHostPortDb.Split("/")[1];
                    var pgUser = pgUserPass.Split(":")[0];
                    var pgPass = pgUserPass.Split(":")[1];
                    var pgHost = pgHostPort.Split(":")[0];
                    var pgPort = pgHostPort.Split(":")[1];

                    connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}; SSL Mode=Require; Trust Server Certificate=true";
                }

                // Whether the connection string came from the local development configuration file
                // or from the environment variable from Heroku, use it to set up your DbContext.
                options.UseNpgsql(connStr);
            });

            // end of database connnection string setting

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