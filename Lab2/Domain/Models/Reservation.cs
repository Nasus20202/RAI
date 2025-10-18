namespace Lab2.Domain.Models;

public class Reservation(Room reservedRoom, User user, DateTime startTime, DateTime endTime)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Room ReservedRoom { get; set; } = reservedRoom;
    public User User { get; set; } = user;
    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;

    public TimeSpan Duration => EndTime - StartTime;
    public bool IsActive => EndTime > DateTime.Now && StartTime <= DateTime.Now;
    public bool IsUpcoming => StartTime > DateTime.Now;
}
