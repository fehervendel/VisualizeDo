using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public class BoardRepository : IBoardRepository
{
    public async Task<IEnumerable<Board>> GetAll()
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Boards.Include(b => b.Lists)
            .ThenInclude(l => l.Cards)
            .ToListAsync();
    }

    public async Task<Board> GetById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Boards
            .Include(b => b.Lists)
            .ThenInclude(l => l.Cards)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task Add(Board board)
    {
        using var dbContext = new VisualizeDoContext();

        var user = await dbContext.Users.FindAsync(board.UserId);

        if (user != null)
        {
            board.User = user;
            dbContext.Boards.Add(board);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("User not found for the provided User Id");
        }
    }

    public async Task Update(Board board)
    {
        using var dbContext = new VisualizeDoContext();
        dbContext.Entry(board).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        var board = await dbContext.Boards.FindAsync(id);
        if (board != null)
        {
            dbContext.Boards.Remove(board);
            await dbContext.SaveChangesAsync();
        }
    }
}