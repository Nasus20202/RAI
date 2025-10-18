using System.Collections.Concurrent;
using Lab2.Application.Interfaces.Repositories;
using Lab2.Domain.Models;
using Lab2.Infrastructure.Exceptions;

namespace Lab2.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<string, User> _users = new();

    public void AddUser(User user)
    {
        if (GetUserByUsername(user.Username) != null)
        {
            throw new UserAlreadyExistsException(user.Username);
        }

        _users.TryAdd(user.Username, user);
    }

    public User? GetUserByUsername(string username)
    {
        return _users.TryGetValue(username, out var user) ? user : null;
    }
}
