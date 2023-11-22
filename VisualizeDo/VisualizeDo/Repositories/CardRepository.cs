using Microsoft.EntityFrameworkCore;
using VisualizeDo.Context;
using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public class CardRepository : ICardRepository
{
    public async Task<IEnumerable<Card>> GetAll()
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Cards.ToListAsync();
    }

    public async Task<Card?> GetById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        return await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task Add(Card card)
    {
        using var dbContext = new VisualizeDoContext();

        var list = await dbContext.Lists.FindAsync(card.ListId);

        if (list != null)
        {
            card.List = list;
            dbContext.Add(card);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("List not found for provided List Id");
        }
    }

    public async Task Update(Card card)
    {
        using var dbContext = new VisualizeDoContext();
        dbContext.Update(card);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteById(int id)
    {
        using var dbContext = new VisualizeDoContext();
        Card? cardToDelete = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == id);
        if (cardToDelete != null)
        {
            dbContext.Remove(cardToDelete);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task ChangeList(int cardId, int listId)
    {
        using var dbContext = new VisualizeDoContext();

        Card? cardToChange = await dbContext.Cards.FirstOrDefaultAsync(c => c.Id == cardId);
        List? targetList = await dbContext.Lists.FirstOrDefaultAsync(l => l.Id == listId);
        if ((cardToChange != null) && (targetList != null))
        {
            cardToChange.ListId = targetList.Id;
            cardToChange.List = targetList;
            dbContext.Update(cardToChange);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            throw new ArgumentException("Card or List not found for provided Id");
        }
    }
}