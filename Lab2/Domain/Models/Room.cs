namespace Lab2.Domain.Models;

public class Room(string Name, int Capacity)
{
    public string Name { get; set; } = Name;
    public int Capacity { get; set; } = Capacity;
}
