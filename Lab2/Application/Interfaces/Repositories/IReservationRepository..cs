using Lab2.Domain.Models;

namespace Lab2.Application.Interfaces.Repositories;

public interface IReservationRepository
{
    void AddReservation(Reservation reservation);
    void RemoveReservation(Reservation reservation);
    IReadOnlyCollection<Reservation> GetAllReservations();
    Reservation? GetReservationById(Guid id);
    IReadOnlyCollection<Reservation> GetReservationsByUser(User user);
    IReadOnlyCollection<Reservation> GetReservationsByRoom(Room room);
}
