using System.Collections.Concurrent;
using Lab2.Application.Interfaces.Repositories;
using Lab2.Domain.Models;

namespace Lab2.Infrastructure.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly ConcurrentDictionary<Guid, Reservation> _reservations = new();

    public void AddReservation(Reservation reservation)
    {
        _reservations.TryAdd(reservation.Id, reservation);
    }

    public void RemoveReservation(Reservation reservation)
    {
        _reservations.TryRemove(reservation.Id, out _);
    }

    public IReadOnlyCollection<Reservation> GetAllReservations()
    {
        return _reservations.Values.ToList().AsReadOnly();
    }

    public Reservation? GetReservationById(Guid id)
    {
        _reservations.TryGetValue(id, out var reservation);
        return reservation;
    }

    public IReadOnlyCollection<Reservation> GetReservationsByUser(User user)
    {
        return _reservations
            .Values.Where(r =>
                r.User.Username.Equals(user.Username, StringComparison.OrdinalIgnoreCase)
            )
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<Reservation> GetReservationsByRoom(Room room)
    {
        return _reservations
            .Values.Where(r =>
                r.ReservedRoom.Name.Equals(room.Name, StringComparison.OrdinalIgnoreCase)
            )
            .ToList()
            .AsReadOnly();
    }
}
