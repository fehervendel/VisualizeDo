using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public class ListRepository : IListRepository
{
    public async Task<IEnumerable<List>> GetAll()
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Lists.Include(l => l.Cards).ToListAsync();
    }

    public async Task<List> GetById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Lists.Include(l => l.Cards).FirstOrDefaultAsync(l => l.Id == id);
    }
    
    public async Task<List<List>> GetByBoardId(int id)
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Lists.Include(l => l.Cards).Where(l => l.BoardId == id).ToListAsync();
    }

    public async Task Add(List list)
    {
        using var dbContext = new VisualizeDoContext();

        var board = await dbContext.Boards.FindAsync(list.BoardId);
        if (board != null)
        {
            list.Board = board;
            dbContext.Lists.Add(list);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("Board not found for the provided Board Id");
        }
    }

    public async Task Update(List list)
    {
        using var dbContext = new VisualizeDoContext();
        dbContext.Entry(list).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        var list = await dbContext.Lists.FindAsync(id);
        if (list != null)
        {
            dbContext.Lists.Remove(list);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task AddLists(List<List> lists)
    {
        using var dbContext = new VisualizeDoContext();
        var board = await dbContext.Boards.FindAsync(lists[0].BoardId);
        if (board != null)
        {
            foreach (var list in lists)
            {
                list.Board = board;
                dbContext.Lists.Add(list);
            }
            await dbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("Board not found for the provided Board Id");
        }
    }
}