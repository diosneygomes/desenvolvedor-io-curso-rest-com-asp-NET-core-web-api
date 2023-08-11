using DevIO.Api.Extensions;

namespace DevIO.Api.Configuration
{
    public static class LoggerConfig
    {
        public static IServiceCollection AddLoggingConfig(this IServiceCollection services, IConfiguration configuration)
        {
            // habilitar somente de uso do Elmah
            //services.AddElmahIo(o =>
            //{
            //    o.ApiKey = "minhaChaveApiKey";
            //    o.LogId = new Guid("meuLogId");
            //});

            services.AddHealthChecks()
                // habilitar somente de uso do Elmah
                //.AddElmahIoPublisher(options =>
                //{
                //    options.ApiKey = "minhaChaveApiKey";
                //    options.LogId = new Guid("meuLogId");
                //    options.HeartbeatId = "API Fornecedores";

                //})
                .AddCheck("Produtos", new SqlServerHealthCheck(configuration.GetConnectionString("DefaultConnection")))
                .AddSqlServer(configuration.GetConnectionString("DefaultConnection"), name: "BancoSQL");

            services.AddHealthChecksUI()
                .AddSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));

            return services;
        }

        public static IApplicationBuilder UseLoggingConfiguration(this IApplicationBuilder app)
        {
            // habilitar somente de uso do Elmah
            //app.UseElmahIo(); 

            return app;
        }
    }
}
