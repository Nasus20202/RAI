using Lab2.Application.DTOs;
using Lab2.Application.Exceptions;
using Lab2.Application.Interfaces.Repositories;
using Lab2.Application.Interfaces.Services;
using Lab2.Domain.Models;

namespace Lab2.Application.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    public RoomService(IRoomRepository roomRepository, IReservationRepository reservationRepository)
    {
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
    }

    public void AddRoom(CreateRoomDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new InvalidRoomNameException("Room name cannot be empty.");
        }

        if (dto.Capacity <= 0)
        {
            throw new InvalidRoomCapacityException("Room capacity must be greater than zero.");
        }

        var room = new Room(dto.Name, dto.Capacity);
        _roomRepository.AddRoom(room);
    }

    public RoomDto? GetRoomByName(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
        {
            throw new InvalidRoomNameException("Room name cannot be empty.");
        }

        var room = _roomRepository.GetRoomByName(roomName);
        if (room == null)
        {
            return null;
        }

        var isAvailable = IsRoomAvailable(roomName);
        return RoomDto.FromRoom(room, isAvailable);
    }

    public IReadOnlyCollection<RoomDto> GetAllRooms()
    {
        var allRooms = _roomRepository.GetAllRooms();
        var allReservations = _reservationRepository.GetAllReservations();

        var occupiedRoomNames = allReservations
            .Where(r => r.IsActive)
            .Select(r => r.ReservedRoom.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return allRooms
            .Select(room => RoomDto.FromRoom(room, !occupiedRoomNames.Contains(room.Name)))
            .ToList()
            .AsReadOnly();
    }

    public IReadOnlyCollection<RoomDto> GetAvailableRooms()
    {
        var allRooms = _roomRepository.GetAllRooms();
        var allReservations = _reservationRepository.GetAllReservations();

        // Get rooms that don't have active reservations
        var occupiedRoomNames = allReservations
            .Where(r => r.IsActive)
            .Select(r => r.ReservedRoom.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return allRooms
            .Where(room => !occupiedRoomNames.Contains(room.Name))
            .Select(room => RoomDto.FromRoom(room, true))
            .ToList()
            .AsReadOnly();
    }

    public bool IsRoomAvailable(string roomName)
    {
        var room = _roomRepository.GetRoomByName(roomName);
        if (room == null)
        {
            return false;
        }

        var reservations = _reservationRepository.GetReservationsByRoom(room);
        return !reservations.Any(r => r.IsActive);
    }

    public bool RoomExists(string roomName)
    {
        return _roomRepository.GetRoomByName(roomName) != null;
    }

    public void DeleteRoom(string roomName)
    {
        if (string.IsNullOrWhiteSpace(roomName))
        {
            throw new InvalidRoomNameException("Room name cannot be empty.");
        }

        var room =
            _roomRepository.GetRoomByName(roomName) ?? throw new RoomNotFoundException(roomName);

        // Remove all reservations associated with this room
        var reservations = _reservationRepository.GetReservationsByRoom(room);
        foreach (var reservation in reservations)
        {
            _reservationRepository.RemoveReservation(reservation);
        }

        // Remove the room
        _roomRepository.RemoveRoom(room);
    }
}
