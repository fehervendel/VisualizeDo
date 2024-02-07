using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Net.Http.Headers;
using VisualizeDo.Contracts;
using VisualizeDo.Models;
using List = VisualizeDo.Models.List;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace VisualizeDoTest;

public class TestPrepare : WebApplicationFactory<Program>
{
    public User? User;
    public Board? Board;
    public List? List;
    public Card? Card;
    private HttpClient _client;
    
    public async Task Prepare()
    {
        _client = CreateClient();
        await InitializeTestUserByEmail();
        await InitializeTestBoard();
        await InitializeTestList();
        await InitializeTestCard();
    }
    

    private async Task InitializeTestUserByEmail()
    {
        User testUser = await CheckTestUser();
        if (testUser == null)
        { 
            await RegisterTestUser("testemail@testemail.com", "testuser", "testpassword");
            User newTestUser = await CheckTestUser();
            User = newTestUser;
        }
        else
        {
            User = testUser;
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
        if (User.Boards == null || User.Boards.Count == 0 || User.Boards[0] == null)
        {
            Board = await CreateTestBoard();
        }
        else
        {
            Board = User.Boards[0];
        }
    }

    private async Task InitializeTestList()
    {
        if (Board.Lists == null || Board.Lists.Count == 0 || Board.Lists[0] == null)
        {
            List = await CreateList();
        }
        else
        {
            List = Board.Lists[0];
        }
    }

    private async Task InitializeTestCard()
    {
        if (List.Cards == null || List.Cards.Count == 0 || List.Cards[0] == null)
        {
            Card = await CreateCard();
        }
        else
        {
            Card = List.Cards[0];
        }
    }
    private async Task<Board?> CreateTestBoard()
    {
        string apiUrl = $"/VisualizeDo/AddBoard?email={User?.EmailAddress}&name=VisualizeDoTestBoard";
        var response = await _client.PostAsync(apiUrl, null);
        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseContent = await response.Content.ReadAsStringAsync(); 
        Board? board = JsonSerializer.Deserialize<Board>(responseContent, options);

        Board = board;
        return board;
    }

    private async Task<List> CreateList()
    {
        string apiUrl = $"/VisualizeDo/AddList?boardId={Board.Id}&name=TestList";
        var response = await _client.PostAsync(apiUrl, null);
        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var responseContent = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<List>(responseContent, options);

        List = list;
        return list;
    }
    
    private async Task<Card?> CreateCard()
    {
        var cardData = new Dictionary<string, object>
        {
            { "listId", List.Id },
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

        Card = card;

        return card;
    }
}