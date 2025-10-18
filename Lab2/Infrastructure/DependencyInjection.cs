using Lab2.Application.Interfaces.Repositories;
using Lab2.Infrastructure.Data;
using Lab2.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IRoomRepository, RoomRepository>();
        services.AddSingleton<IReservationRepository, ReservationRepository>();
        services.AddTransient<IDataInitializer, DataInitializer>();
        return services;
    }
}
