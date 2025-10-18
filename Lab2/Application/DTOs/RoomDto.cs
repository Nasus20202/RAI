using Lab2.Domain.Models;

namespace Lab2.Application.DTOs;

public record RoomDto(string Name, int Capacity, bool IsAvailable)
{
    public static RoomDto FromRoom(Room room, bool isAvailable = true)
    {
        return new RoomDto(room.Name, room.Capacity, isAvailable);
    }
}
