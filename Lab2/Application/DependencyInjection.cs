using Lab2.Application.Interfaces.Services;
using Lab2.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Lab2.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoomService, RoomService>();
        services.AddSingleton<IBookingService, BookingService>();
        return services;
    }
}
