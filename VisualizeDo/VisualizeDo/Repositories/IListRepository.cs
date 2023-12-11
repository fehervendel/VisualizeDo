using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public interface IListRepository
{
    Task<IEnumerable<List>> GetAll();
    Task<List> GetById(int id);
    Task Add(List list);
    Task Update(List list);
    Task DeleteById(int id);
    Task<List<List>> GetByBoardId(int id);
}