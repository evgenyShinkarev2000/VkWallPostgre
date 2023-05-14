using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VkWallPostgre.Data;
using VkWallPostgre.Infostracture;

namespace VkWallPostgre
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetPostgreConnectionString();
                options.UseNpgsql(connectionString);
            });
            builder.Services.AddTransient<VkOpenIdHelper>(factory =>
            {
                return new VkOpenIdHelper(builder.Configuration.GetOpenIdHelperParams());
            });
            builder.Services.AddSingleton<ISimpleLogger, FileLogger>(factory =>
            {
                return new FileLogger("logs.log");
            });
            builder.Services.AddDistributedMemoryCache();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}