using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using Sprache;
using VisualizeDo.Context;
using VisualizeDo.Contracts;
using VisualizeDo.Controllers;
using VisualizeDo.Models;
using VisualizeDo.Models.DTOs;
using VisualizeDo.Repositories;
using VisualizeDo.Services.Authentication;
using List = VisualizeDo.Models.List;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace VisualizeDoTest;
[TestFixture]
public class VisualizeDoTest : WebApplicationFactory<Program>
{
    private HttpClient _client;
    private VisualizeDoContext _context;
    private User? _user;
    private Board? _board;
    private List? _list;
    private Card? _card;

    [SetUp]
    public async Task Setup()
    {
        DotNetEnv.Env.Load();
        string connectionString = DotNetEnv.Env.GetString("CONNECTION_STRING");
        Environment.SetEnvironmentVariable("CONNECTION_STRING", "Server=localhost,1433;Database=VisualizeDo;User Id=sa;Password=Feher2023vendeL!;");

        _client = CreateClient();
        await InitializeTestUserByEmail();
        await InitializeTestBoard();
        await InitializeTestList();
        await InitializeTestCard();
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        if (_client != null)
        {
            _client.Dispose();
        }

        if (_context != null)
        {
             _context.Dispose();
        }
    }

    private async Task InitializeTestUserByEmail()
    {
        User testUser = await CheckTestUser();
        if (testUser == null)
        { 
            await RegisterTestUser("testemail@testemail.com", "testuser", "testpassword");
            User newTestUser = await CheckTestUser();
            _user = newTestUser;
        }
        else
        {
            _user = testUser;
        }
    }

    private async Task<User> CheckTestUser()
    {
        try
        {
            var response = await _client.GetAsync("/VisualizeDo/GetUserByEmail?email=testemail@testemail.com");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            User? user = JsonSerializer.Deserialize<User>(content, options);
            return user;
        }
        catch (HttpRequestException ex)
        {
            return null;
        }
    }

    private async Task RegisterTestUser(string email, string username, string password)
    {
        var registrationRequest = new RegistrationRequest(email, username, password);
        var jsonString = JsonSerializer.Serialize(registrationRequest);
        var jsonStringContent = new StringContent(jsonString);
        jsonStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        await _client.PostAsync("/Auth/Register", jsonStringContent);
    }

    private async Task InitializeTestBoard()
    {
        if (_user.Boards == null || _user.Boards.Count == 0 || _user.Boards[0] == null)
        {
            _board = await CreateTestBoard();
        }
        else
        {
            _board = _user.Boards[0];
        }
    }

    private async Task InitializeTestList()
    {
        if (_board.Lists == null || _board.Lists.Count == 0 || _board.Lists[0] == null)
        {
            _list = await CreateList();
        }
        else
        {
            _list = _board.Lists[0];
        }
    }

    private async Task InitializeTestCard()
    {
        if (_list.Cards == null || _list.Cards.Count == 0 || _list.Cards[0] == null)
        {
            _card = await CreateCard();
        }
        else
        {
            _card = _list.Cards[0];
        }
    }
    private async Task<Board?> CreateTestBoard()
    {
        string apiUrl = $"/VisualizeDo/AddBoard?email={_user?.EmailAddress}&name=VisualizeDoTestBoard";
        var response = await _client.PostAsync(apiUrl, null);
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = await response.Content.ReadAsStringAsync(); 
        Board? board = JsonSerializer.Deserialize<Board>(responseContent, options);

        _board = board;
        return board;
    }

    private async Task<List> CreateList()
    {
        string apiUrl = $"/VisualizeDo/AddList?boardId={_board.Id}&name=TestList";
        var response = await _client.PostAsync(apiUrl, null);
        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var responseContent = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(responseContent, options);

        _list = list;
        return list;
    }
    
    private async Task<Card?> CreateCard()
    {
        var cardData = new Dictionary<string, object>
        {
            { "listId", _list.Id },
            { "title", "TestTitle" },
            { "description", "TestDescription" },
            { "priority", "Urgent" },
            { "size", "Large" }
        };
        
        var jsonData = JsonSerializer.Serialize(cardData);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
        
        string apiUrl = $"/VisualizeDo/AddCard";
        var response = await _client.PostAsync(apiUrl, content);
        
        response.EnsureSuccessStatusCode();
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var responseContent = await response.Content.ReadAsStringAsync();
        var card = JsonSerializer.Deserialize<Card>(responseContent, options);

        _card = card;

        return card;
    }
    
    [Test]
    public void Test1()
    {
    }
}