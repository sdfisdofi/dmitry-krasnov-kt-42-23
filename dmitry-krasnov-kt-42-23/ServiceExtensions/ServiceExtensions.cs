using dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces;

namespace dmitry_krasnov_kt_42_23.ServiceExtensions
{
    public static class ServiceExtensions
    {
        // Регистрация сервисов: связываем интерфейс с его реализацией (Dependency Injection)
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IGroupService, GroupService>();

            return services;
        }
    }
}
