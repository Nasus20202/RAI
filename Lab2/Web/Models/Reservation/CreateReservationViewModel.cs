using System.ComponentModel.DataAnnotations;
using Lab2.Application.DTOs;

namespace Lab2.Web.Models.Reservation;

public class CreateReservationViewModel
{
    [Required(ErrorMessage = "Please select a room")]
    [Display(Name = "Room")]
    public string? RoomName { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    [Display(Name = "Start Time")]
    [DataType(DataType.DateTime)]
    public DateTime? StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    [Display(Name = "End Time")]
    [DataType(DataType.DateTime)]
    public DateTime? EndTime { get; set; }

    public IEnumerable<RoomDto> AvailableRooms { get; set; } = new List<RoomDto>();

    public string? ErrorMessage { get; set; }
}
