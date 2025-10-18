using Lab2.Application.DTOs;
using Lab2.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    : ControllerBase
{
    private readonly IRoomService _roomService = roomService;
    private readonly ILogger<RoomsController> _logger = logger;

    /// <summary>
    /// Get all rooms
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<RoomDto>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<RoomDto>> GetAllRooms()
    {
        _logger.LogInformation("API: Getting all rooms");
        var rooms = _roomService.GetAllRooms();
        return Ok(rooms);
    }

    /// <summary>
    /// Get specific room by name
    /// </summary>
    [HttpGet("{name}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<RoomDto> GetRoom(string name)
    {
        _logger.LogInformation("API: Getting room {RoomName}", name);
        var room = _roomService.GetRoomByName(name);

        if (room == null)
        {
            return NotFound(new { error = "Room not found" });
        }

        return Ok(room);
    }

    /// <summary>
    /// Create a new room (Admin only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<RoomDto> CreateRoom([FromBody] CreateRoomDto dto)
    {
        _logger.LogInformation(
            "API: Admin creating room {RoomName} with capacity {Capacity}",
            dto.Name,
            dto.Capacity
        );

        _roomService.AddRoom(dto);
        var room = _roomService.GetRoomByName(dto.Name);

        _logger.LogInformation("API: Room {RoomName} created successfully", dto.Name);

        return CreatedAtAction(nameof(GetRoom), new { name = dto.Name }, room);
    }

    /// <summary>
    /// Delete a room by name (Admin only)
    /// </summary>
    [HttpDelete("{roomName}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult DeleteRoom(string roomName)
    {
        _logger.LogInformation("API: Admin deleting room {RoomName}", roomName);

        _roomService.DeleteRoom(roomName);

        _logger.LogInformation("API: Room {RoomName} deleted successfully", roomName);

        return NoContent();
    }
}
