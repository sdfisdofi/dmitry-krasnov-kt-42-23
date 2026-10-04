using dmitry_krasnov_kt_42_23.Interfaces.DisciplinesInterfaces;
using dmitry_krasnov_kt_42_23.Interfaces.GroupsInterfaces;
using dmitry_krasnov_kt_42_23.Interfaces.PerformanceInterfaces;
using dmitry_krasnov_kt_42_23.Interfaces.StudentsInterfaces;

namespace dmitry_krasnov_kt_42_23.ServiceExtensions
{
    public static class ServiceExtensions
    {
        // Регистрация сервисов: связываем интерфейс с его реализацией (Dependency Injection)
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IDisciplineService, DisciplineService>();
            services.AddScoped<IPerformanceService, PerformanceService>();

            return services;
        }
    }
}
