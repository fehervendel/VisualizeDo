using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using VisualizeDo.Models;
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
                    response.EnsureSuccessStatusCode();
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
}