using Microsoft.EntityFrameworkCore;
using Payments.Repository;
using Payments.Services;

namespace Payments.Api;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add services to the container.
        services.AddDbContext<PaymentDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        // Register generic repository
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register services
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IAggregatedPaymentService, AggregatedPaymentService>();

        services.AddControllers();
        services.AddOpenApi();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Ensure database is created and migrations are applied
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            context.Database.Migrate();
        }

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapOpenApi());
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
