using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public interface IBoardRepository
{
    Task<IEnumerable<Board>> GetAll();
    Task<Board> GetById(int id);
    Task Add(Board board);
    Task Update(Board board);
    Task DeleteById(int id);
}