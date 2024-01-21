using FluentValidation;
using NurediniCargoAPI.Data;
using NurediniCargoAPI.Entities;
using NurediniCargoAPI.Repositories.Interfaces;
using NurediniCargoAPI.Repositories;
using NurediniCargoAPI.Validators;
using Microsoft.EntityFrameworkCore;

namespace NurediniCargoAPI
{
    public class Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        public IConfiguration Configuration { get; } = configuration;
        public IWebHostEnvironment Environment { get; } = environment;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            services.AddControllers();

            services.AddHttpClient();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            // Add Repositories
            services.AddScoped<IAsyncRepository<Goods>, RepositoryBase<Goods>>();
            services.AddScoped<IAsyncRepository<Supplier>, RepositoryBase<Supplier>>();
            services.AddScoped<IAsyncRepository<Warehouse>, RepositoryBase<Warehouse>>();

            services.AddScoped<IAsyncRepository<Inventory>, InventoryRepository>();
            services.AddScoped<IMovementRepository, MovementRepository>();

            //Add Validators
            services.AddScoped<IValidator<Goods>, GoodsValidator>();
            services.AddScoped<IValidator<Supplier>, SupplierValidator>();
            services.AddScoped<IValidator<Warehouse>, WarehouseValidator>();

            services.AddScoped<IValidator<Inventory>, InventoryValidator>();
            services.AddScoped<IValidator<Movement>, MovementValidator>();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
