using System.Collections.Concurrent;
using System.Collections.Immutable;
using Lab2.Application.Interfaces.Repositories;
using Lab2.Domain.Models;
using Lab2.Infrastructure.Exceptions;

namespace Lab2.Infrastructure.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly ConcurrentDictionary<string, Room> _rooms = new();

    public void AddRoom(Room room)
    {
        if (GetRoomByName(room.Name) != null)
        {
            throw new RoomAlreadyExistsException(room.Name);
        }

        _rooms.TryAdd(room.Name, room);
    }

    public IReadOnlyCollection<Room> GetAllRooms()
    {
        return _rooms.Values.ToList().AsReadOnly();
    }

    public Room? GetRoomByName(string name)
    {
        return _rooms.TryGetValue(name, out var room) ? room : null;
    }

    public void RemoveRoom(Room room)
    {
        _rooms.TryRemove(room.Name, out _);
    }
}
