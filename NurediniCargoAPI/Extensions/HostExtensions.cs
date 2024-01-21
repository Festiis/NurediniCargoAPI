using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;


namespace NurediniCargoAPI.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, Action<TContext> seeder) where TContext: DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();

                try
                {
                    Console.WriteLine("Migrating database associated with context");

                    var retry = Policy.Handle<SqlException>()
                            .WaitAndRetry(
                                retryCount: 5,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // 2,4,8,16,32 sc
                                onRetry: (exception, retryCount, context) =>
                                {
                                    Console.WriteLine($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
                                });

                    //if the sql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for DbContext only 
                    //apply to transient exceptions                    
                    retry.Execute(() => InvokeSeeder(seeder, context));

                    Console.WriteLine("Migrated database associated with context");
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"An error occurred while migrating the database used on context {ex.Message}");
                }


            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext> seeder, TContext context)
            where TContext : DbContext
        {
            context.Database.Migrate();
            seeder(context);
        }
    }
}
