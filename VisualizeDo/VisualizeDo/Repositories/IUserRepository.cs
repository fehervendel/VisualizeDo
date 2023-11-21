using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetById(int id);
    Task Add(User user);
    Task Delete(int id);
    Task Update(User user);
}