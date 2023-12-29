using VisualizeDo.Models;

namespace VisualizeDo.Repositories;

public interface ICardRepository
{
    Task<IEnumerable<Card>> GetAll();
    Task<Card> GetById(int id);
    Task Add(Card card);
    Task Update(Card card);
    Task DeleteById(int id);
    Task ChangeList(int cardId, int listId);
    Task EditCard(int id, string title, string description, string priority, string size);
}