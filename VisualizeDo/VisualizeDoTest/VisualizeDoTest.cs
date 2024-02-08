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

    private TestPrepare _testPrepare = new TestPrepare();

    [SetUp]
    public async Task Setup()
    {
        DotNetEnv.Env.Load();
        string connectionString = DotNetEnv.Env.GetString("CONNECTION_STRING");
        Environment.SetEnvironmentVariable("CONNECTION_STRING",
            "Server=localhost,1433;Database=VisualizeDo;User Id=sa;Password=Feher2023vendeL!;");

        _client = CreateClient();
        await _testPrepare.Prepare();
        _user = _testPrepare.User;
        _board = _testPrepare.Board;
        _list = _testPrepare.List;
        _card = _testPrepare.Card;
        // TestContext.Progress.WriteLine(_user.Name);
        // TestContext.Progress.WriteLine(_board.Name);
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
        //Assert.That(user.Boards[0].Lists[0].Cards[0], Is.DeepEqualTo(_user.Boards[0].Lists[0].Cards[0])); //HOW
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
}