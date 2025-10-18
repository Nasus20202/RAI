using Lab2.Application.Interfaces.Repositories;
using Lab2.Domain.Models;

namespace Lab2.Infrastructure.Data;

public class DataInitializer : IDataInitializer
{
    private readonly IUserRepository _userRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IReservationRepository _reservationRepository;

    public DataInitializer(
        IUserRepository userRepository,
        IRoomRepository roomRepository,
        IReservationRepository reservationRepository
    )
    {
        _userRepository = userRepository;
        _roomRepository = roomRepository;
        _reservationRepository = reservationRepository;
    }

    public void Initialize()
    {
        // Initialize Users
        InitializeUsers();

        // Initialize Rooms
        InitializeRooms();

        // Initialize Reservations
        InitializeReservations();
    }

    private void InitializeUsers()
    {
        // Create admin user
        var admin = new User(
            Username: "admin",
            PasswordHash: HashPassword("admin123"),
            UserRole: Role.Admin
        );
        _userRepository.AddUser(admin);

        // Create regular users
        var users = new[]
        {
            new User("john.doe", HashPassword("password123")),
            new User("jane.smith", HashPassword("password123")),
            new User("bob.wilson", HashPassword("password123")),
            new User("alice.brown", HashPassword("password123")),
            new User("charlie.davis", HashPassword("password123")),
        };

        foreach (var user in users)
        {
            _userRepository.AddUser(user);
        }
    }

    private void InitializeRooms()
    {
        var rooms = new[]
        {
            new Room("Conference Room A", 10),
            new Room("Conference Room B", 8),
            new Room("Meeting Room 1", 6),
            new Room("Meeting Room 2", 6),
            new Room("Board Room", 15),
            new Room("Training Room", 20),
            new Room("Small Office 1", 4),
            new Room("Small Office 2", 4),
            new Room("Auditorium", 50),
            new Room("Workshop Space", 12),
        };

        foreach (var room in rooms)
        {
            _roomRepository.AddRoom(room);
        }
    }

    private void InitializeReservations()
    {
        // Get some users and rooms for creating sample reservations
        var johnDoe = _userRepository.GetUserByUsername("john.doe");
        var janeSmith = _userRepository.GetUserByUsername("jane.smith");
        var bobWilson = _userRepository.GetUserByUsername("bob.wilson");

        var conferenceRoomA = _roomRepository.GetRoomByName("Conference Room A");
        var meetingRoom1 = _roomRepository.GetRoomByName("Meeting Room 1");
        var boardRoom = _roomRepository.GetRoomByName("Board Room");

        // Past completed reservation
        if (johnDoe != null && conferenceRoomA != null)
        {
            var reservation1 = new Reservation(
                conferenceRoomA,
                johnDoe,
                DateTime.Now.AddDays(-2),
                DateTime.Now.AddDays(-1)
            );
            _reservationRepository.AddReservation(reservation1);
        }

        // Active reservation (ongoing)
        if (janeSmith != null && meetingRoom1 != null)
        {
            var reservation2 = new Reservation(
                meetingRoom1,
                janeSmith,
                DateTime.Now.AddHours(-3),
                DateTime.Now.AddHours(2)
            );
            _reservationRepository.AddReservation(reservation2);
        }

        // Past completed reservation
        if (bobWilson != null && boardRoom != null)
        {
            var reservation3 = new Reservation(
                boardRoom,
                bobWilson,
                DateTime.Now.AddDays(-5),
                DateTime.Now.AddDays(-4)
            );
            _reservationRepository.AddReservation(reservation3);
        }

        // Upcoming reservation
        if (johnDoe != null && boardRoom != null)
        {
            var reservation4 = new Reservation(
                boardRoom,
                johnDoe,
                DateTime.Now.AddHours(1),
                DateTime.Now.AddHours(3)
            );
            _reservationRepository.AddReservation(reservation4);
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
