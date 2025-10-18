using Lab2.Domain.Models;

namespace Lab2.Application.Interfaces.Repositories;

public interface IRoomRepository
{
    void AddRoom(Room room);
    void RemoveRoom(Room room);
    IReadOnlyCollection<Room> GetAllRooms();
    Room? GetRoomByName(string name);
}
