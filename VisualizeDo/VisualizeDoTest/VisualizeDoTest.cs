using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Serialization;
using VisualizeDo.Models;
using VisualizeDo.Models.DTOs;
using List = VisualizeDo.Models.List;

namespace VisualizeDoTest;

[TestFixture]
public class VisualizeDoTest : WebApplicationFactory<Program>
{
    private HttpClient _client;

    private User? _user;
    private Board? _board;
    private List? _list;
    private Card? _card;

    private JsonSerializerOptions _options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly TestPrepare _testPrepare = new TestPrepare();

    [SetUp]
    public async Task Setup()
    {
        DotNetEnv.Env.Load();
        string connectionString = DotNetEnv.Env.GetString("CONNECTION_STRING");
        Environment.SetEnvironmentVariable("CONNECTION_STRING",
            "Server=localhost,1433;Database=VisualizeDo;User Id=sa;Password=Feher2023vendeL!;");

        _client = CreateClient();

        if (_user != null)
        {
            string apiUrl = $"/VisualizeDo/DeleteUserById?id={_user.Id}";
                    var response = await _client.DeleteAsync(apiUrl);
        }

        await _testPrepare.Prepare();
        _user = _testPrepare.User;
        _board = _testPrepare.Board;
        _list = _testPrepare.List;
        _card = _testPrepare.Card;

    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
    }
    
    [Test]
    public async Task GetUserByEmailReturnsOk()
    {
        var apiUrl = $"/VisualizeDo/GetUserByEmail?email={_user.EmailAddress}";
        var response = await _client.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();


        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.IsNotNull(user);
        Assert.That(user.Id, Is.EqualTo(_user.Id));
        Assert.That(user.Name, Is.EqualTo(_user.Name));
    }

    [Test]
    public async Task GetUserByEmailReturnsNotFound()
    {
        string email = "123@123.com";
        var apiUrl = $"/VisualizeDo/GetUserByEmail?email={email}";
        var response = await _client.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NoContent));
    }

    [Test]
    public async Task AddNewBoardShouldReturnOk()
    {
        string boardName = "AddBoardTest";
        var apiUrl = $"/VisualizeDo/AddBoard?email={_user.EmailAddress}&name={boardName}";
        var response = await _client.PostAsync(apiUrl, null);
        response.EnsureSuccessStatusCode();

        var userResponse = await _client.GetAsync("/VisualizeDo/GetUserByEmail?email=testemail@testemail.com");
        var userContent = await userResponse.Content.ReadAsStringAsync();
        User? user = JsonSerializer.Deserialize<User>(userContent, _options);
        _user = user;

        var content = await response.Content.ReadAsStringAsync();
        var board = JsonSerializer.Deserialize<Board>(content, _options);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.IsNotNull(board);
        Assert.That(board.Name, Is.EqualTo(boardName));
        Assert.IsTrue(_user.Boards.Any(b => b.Id == board.Id));

        await AddNewBoardShouldReturnOk_CleanUp(board.Id);
    }
    private async Task AddNewBoardShouldReturnOk_CleanUp(int boardId)
    {
        var apiUrl = $"/VisualizeDo/DeleteBoardById?id={boardId}";
        var response = await _client.DeleteAsync(apiUrl);
        response.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task AddNewBoardShouldReturnNotFound()
    {
        string boardName = "AddBoardTestNotFound";
        var apiUrl = $"/VisualizeDo/AddBoard?email={-1}&name={boardName}";
        var response = await _client.PostAsync(apiUrl, null);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetBoardByIdShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/GetBoardById?id={_board.Id}";
        var response = await _client.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var board = JsonSerializer.Deserialize<Board>(content, _options);
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.IsNotNull(board);
        Assert.That(board.Id, Is.EqualTo(_board.Id));
        Assert.That(board.UserId, Is.EqualTo(_board.UserId));
        Assert.That(board.Name, Is.EqualTo(_board.Name));
        Assert.That(board.User, Is.EqualTo(_board.User));
        Assert.That(board.Lists.Count, Is.EqualTo(_board.Lists.Count));
    }
    
    [Test]
    public async Task GetBoardByIdShouldReturnNoContent()
    {
        var apiUrl = $"/VisualizeDo/GetBoardById?id={-1}";
        var response = await _client.GetAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NoContent));
    }
    
    [Test]
    public async Task DeleteBoardByIdShouldReturnOk()
    {
        var userBoardsLength = _user.Boards.Count;
        var apiUrl = $"/VisualizeDo/DeleteBoardById?id={_board.Id}";
        var response = await _client.DeleteAsync(apiUrl);

        response.EnsureSuccessStatusCode();
        
        var userApiUrl = $"/VisualizeDo/GetUserByEmail?email={_user.EmailAddress}";
        var userResponse = await _client.GetAsync(userApiUrl);

        userResponse.EnsureSuccessStatusCode();


        var userContent = await userResponse.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<User>(userContent, _options);
        

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(user.Boards.Count, Is.EqualTo(userBoardsLength - 1));
    }
    
    [Test]
    public async Task DeleteBoardByIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/DeleteBoardById?id={-1}";
        var response = await _client.DeleteAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task ChangeBoardNameShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/ChangeBoardName?id={_board.Id}&newName=BoardNameChangeTest";
        var response = await _client.PutAsync(apiUrl, null);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var board = JsonSerializer.Deserialize<Board>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(board.Name, Is.EqualTo("BoardNameChangeTest"));
    }
    
    [Test]
    public async Task ChangeBoardNameShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/ChangeBoardName?id={-1}&newName=BoardNameChangeTest";
        var response = await _client.PutAsync(apiUrl, null);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task AddListShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/AddList?boardId={_board.Id}&name=AddListTest";
        var response = await _client.PostAsync(apiUrl, null);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(list.Name, Is.EqualTo("AddListTest"));

        var cleanUpUrl = $"/VisualizeDo/DeleteListById?id={list.Id}";
        var cleanUpResponse = await _client.DeleteAsync(cleanUpUrl);
        cleanUpResponse.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task AddListShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/AddList?boardId={-1}&name=AddListTest";
        var response = await _client.PostAsync(apiUrl, null);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task AddListsShouldReturnOk()
    {
        List<string> testLists = new List<string>
        {
            new string("AddListsTest1"),
            new string("AddListsTest2"),
            new string("AddListsTest3")
        };
        
        string json = JsonSerializer.Serialize(testLists);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var apiUrl = $"/VisualizeDo/AddLists?boardId={_board.Id}";
        var response = await _client.PostAsync(apiUrl, content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        List<List> returnedLists = JsonSerializer.Deserialize<List<List>>(responseContent, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(returnedLists.Count, Is.EqualTo(testLists.Count));
        Assert.That(returnedLists.Any(l => l.Name == testLists[0]), Is.True);
        Assert.That(returnedLists.Any(l => l.Name == testLists[1]), Is.True);
        Assert.That(returnedLists.Any(l => l.Name == testLists[2]), Is.True);

        for (int i = 0; i < testLists.Count; i++)
        {
            var cleanUpUrl = $"/VisualizeDo/DeleteListById?id={returnedLists[i].Id}";
            var cleanUpResponse = await _client.DeleteAsync(cleanUpUrl);

            cleanUpResponse.EnsureSuccessStatusCode();
        }
    }
    
    [Test]
    public async Task AddListsShouldReturnError()
    {
        List<string> testLists = new List<string>
        {
            new string("AddListsTest1"),
            new string("AddListsTest2"),
            new string("AddListsTest3")
        };
        
        string json = JsonSerializer.Serialize(testLists);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var apiUrl = $"/VisualizeDo/AddLists?boardId={-1}";
        var response = await _client.PostAsync(apiUrl, content);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetListByIdShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/GetListById?id={_list.Id}";
        var response = await _client.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(list.Name, Is.EqualTo(_list.Name));
        Assert.That(list.Cards.Count, Is.EqualTo(_list.Cards.Count));
    }
    
    [Test]
    public async Task GetListByIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/GetListById?id={-1}";
        var response = await _client.GetAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetListsByBoardIdShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/GetListsByBoardId?id={_board.Id}";
        var response = await _client.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var lists = JsonSerializer.Deserialize<List<List>>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.IsNotNull(lists);
        Assert.That(lists.Count, Is.EqualTo(_board.Lists.Count));
    }
    
    [Test]
    public async Task GetListsByBoardIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/GetListsByBoardId?id={-1}";
        var response = await _client.GetAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task DeleteListByIdShouldReturnOk()
    {
        int listsCount = _board.Lists.Count;
        var apiUrl = $"/VisualizeDo/DeleteListById?id={_list.Id}";
        var response = await _client.DeleteAsync(apiUrl);

        response.EnsureSuccessStatusCode();

        var boardApiUrl = $"/VisualizeDo/GetBoardById?id={_board.Id}";
        var boardResponse = await _client.GetAsync(boardApiUrl);

        response.EnsureSuccessStatusCode();

        var boardContent = await boardResponse.Content.ReadAsStringAsync();
        Board? board = JsonSerializer.Deserialize<Board>(boardContent, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(board.Lists.Count, Is.EqualTo(listsCount - 1));
    }
    
    [Test]
    public async Task DeleteListByIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/DeleteListById?id={-1}";
        var response = await _client.DeleteAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task AddCardShouldReturnOk()
    {
        var CardDTO = new AddCard(_list.Id, "AddCardTest", "AddCardTest", "Urgent", "Tiny");
        string json = JsonSerializer.Serialize(CardDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var apiUrl = $"/VisualizeDo/AddCard";
        var response = await _client.PostAsync(apiUrl, content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        Card? card = JsonSerializer.Deserialize<Card>(responseContent, _options);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(card.ListId, Is.EqualTo(_list.Id));
        Assert.That(card.Title, Is.EqualTo("AddCardTest"));
        Assert.That(card.Description, Is.EqualTo("AddCardTest"));
        Assert.That(card.Priority, Is.EqualTo("Urgent"));
        Assert.That(card.Size, Is.EqualTo("Tiny"));

        var cleanUpUrl = $"/VisualizeDo/DeleteCardById?id={card.Id}";
        var cleanUpResponse = await _client.DeleteAsync(cleanUpUrl);

        cleanUpResponse.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task AddCardShouldReturnNotFound()
    {
        var CardDTO = new AddCard(-1, "AddCardTest", "AddCardTest", "Urgent", "Tiny");
        string json = JsonSerializer.Serialize(CardDTO);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var apiUrl = $"/VisualizeDo/AddCard";
        var response = await _client.PostAsync(apiUrl, content);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetCardByIdShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/GetCardById?id={_card.Id}";
        var response = await _client.GetAsync(apiUrl);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var card = JsonSerializer.Deserialize<Card>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(card.Title, Is.EqualTo(_card.Title));
        Assert.That(card.Id, Is.EqualTo(_card.Id));
        Assert.That(card.Description, Is.EqualTo(_card.Description));
        Assert.That(card.Priority, Is.EqualTo(_card.Priority));
        Assert.That(card.Size, Is.EqualTo(_card.Size));
        Assert.That(card.ListId, Is.EqualTo(_card.ListId));
    }
    
    [Test]
    public async Task GetCardByIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/GetCardById?id={-1}";
        var response = await _client.GetAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task ChangeCardsListByIdShouldReturnOk()
    {
        var listApiUrl = $"/VisualizeDo/AddList?boardId={_board.Id}&name=ChangeCardsListTest";
        var listResponse = await _client.PostAsync(listApiUrl, null);

        listResponse.EnsureSuccessStatusCode();
        
        var listContent = await listResponse.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(listContent, _options);
        
        
        var apiUrl = $"/VisualizeDo/ChangeCardsListById?cardId={_card.Id}&listId={list.Id}";
        var response = await _client.PutAsync(apiUrl, null);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var card = JsonSerializer.Deserialize<Card>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(card.ListId, Is.EqualTo(list.Id));
        
        var cleanUpUrl = $"/VisualizeDo/DeleteListById?id={list.Id}";
        var cleanUpResponse = await _client.DeleteAsync(cleanUpUrl);
        cleanUpResponse.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task ChangeCardsListByIdShouldReturnNotFound()
    {
        var listApiUrl = $"/VisualizeDo/AddList?boardId={_board.Id}&name=ChangeCardsListTest";
        var listResponse = await _client.PostAsync(listApiUrl, null);

        listResponse.EnsureSuccessStatusCode();
        
        var listContent = await listResponse.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(listContent, _options);
        
        
        var apiUrl = $"/VisualizeDo/ChangeCardsListById?cardId={-1}&listId={-1}";
        var response = await _client.PutAsync(apiUrl, null);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
        
        var cleanUpUrl = $"/VisualizeDo/DeleteListById?id={list.Id}";
        var cleanUpResponse = await _client.DeleteAsync(cleanUpUrl);
        cleanUpResponse.EnsureSuccessStatusCode();
    }
    
    [Test]
    public async Task ChangeListNameShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/ChangeListName?listId={_list.Id}&newName=ChangeListNameTest";
        var response = await _client.PutAsync(apiUrl, null);

        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(content, _options);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(list.Name, Is.EqualTo("ChangeListNameTest"));
    }
    
    [Test]
    public async Task ChangeListNameShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/ChangeListName?listId={-1}&newName=ChangeListNameTest";
        var response = await _client.PutAsync(apiUrl, null);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }

    [Test]
    public async Task DeleteCardByIdShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/DeleteCardById?id={_card.Id}";
        var response = await _client.DeleteAsync(apiUrl);

        response.EnsureSuccessStatusCode();

        var listApiUrl = $"/VisualizeDo/GetListById?id={_list.Id}";
        var listResponse = await _client.GetAsync(listApiUrl);

        listResponse.EnsureSuccessStatusCode();

        var listResponseContent = await listResponse.Content.ReadAsStringAsync();
        List? list = JsonSerializer.Deserialize<List>(listResponseContent);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(list.Cards, Is.EqualTo(null));
    }
    
    [Test]
    public async Task DeleteCardByIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/DeleteCardById?id={-1}";
        var response = await _client.DeleteAsync(apiUrl);

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task DeleteUserByIdShouldReturnOk()
    {
        var apiUrl = $"/VisualizeDo/DeleteUserById?id={_user.Id}";
        var response = await _client.DeleteAsync(apiUrl);

        response.EnsureSuccessStatusCode();

        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
    }
    
    [Test]
    public async Task DeleteUserByIdShouldReturnNotFound()
    {
        var apiUrl = $"/VisualizeDo/DeleteUserById?id={-1}";
        var response = await _client.DeleteAsync(apiUrl);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task EditCardShouldReturnOk()
    {
        EditCard editData = new EditCard
        (
            _card.Id,
            "EditCardTest",
            "EditCardTestDescription", 
            "Low",
            "Small"
        );
        var jsonContent = JsonSerializer.Serialize(editData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var apiUrl = $"/VisualizeDo/EditCard";
        var response = await _client.PutAsync(apiUrl, content);
    
        response.EnsureSuccessStatusCode();
        
        var cardApiUrl = $"/VisualizeDo/GetCardById?id={_card.Id}";
        var cardResponse = await _client.GetAsync(cardApiUrl);
    
        response.EnsureSuccessStatusCode();
        
        var cardContent = await cardResponse.Content.ReadAsStringAsync();
        var card = JsonSerializer.Deserialize<Card>(cardContent, _options);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
        Assert.That(card.Id, Is.EqualTo(_card.Id));
        Assert.That(card.Title, Is.EqualTo("EditCardTest"));
        Assert.That(card.Description, Is.EqualTo("EditCardTestDescription"));
        Assert.That(card.Priority, Is.EqualTo("Low"));
        Assert.That(card.Size, Is.EqualTo("Small"));
    }
    
    [Test]
    public async Task EditCardShouldReturnNotFound()
    {
        EditCard editData = new EditCard
        (
            -1,
            "EditCardTest",
            "EditCardTestDescription", 
            "Low",
            "Small"
        );
        var jsonContent = JsonSerializer.Serialize(editData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var apiUrl = $"/VisualizeDo/EditCard";
        var response = await _client.PutAsync(apiUrl, content);
        
        Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));
    }
}