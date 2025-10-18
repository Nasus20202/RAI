using Lab2.Domain.Models;

namespace Lab2.Application.Interfaces.Repositories;

public interface IUserRepository
{
    void AddUser(User user);
    User? GetUserByUsername(string username);
}
