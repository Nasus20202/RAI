using Lab2.Application.DTOs;

namespace Lab2.Application.Interfaces.Services;

public interface IRoomService
{
    void AddRoom(CreateRoomDto dto);
    void DeleteRoom(string roomName);
    RoomDto? GetRoomByName(string roomName);
    IReadOnlyCollection<RoomDto> GetAllRooms();
    IReadOnlyCollection<RoomDto> GetAvailableRooms();
    bool IsRoomAvailable(string roomName);
    bool RoomExists(string name);
}
