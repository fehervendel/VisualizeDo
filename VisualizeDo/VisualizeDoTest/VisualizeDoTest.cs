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
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace VisualizeDoTest;
[TestFixture]
public class VisualizeDoTest : WebApplicationFactory<Program>
{
    private HttpClient _client;
    private int _userId;
    private VisualizeDoContext _context;

    [SetUp]
    public async Task Setup()
    {
        DotNetEnv.Env.Load();
        string connectionString = DotNetEnv.Env.GetString("CONNECTION_STRING");
        Environment.SetEnvironmentVariable("CONNECTION_STRING", connectionString);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _client = CreateClient();
        AuthRequest authRequest = new AuthRequest("admin@admin.com", "admin123");
        string jsonString = JsonSerializer.Serialize(authRequest);
        StringContent jsonStringContent = new StringContent(jsonString);
        jsonStringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = _client.PostAsync("Auth/Login", jsonStringContent).Result;
        var content = response.Content.ReadAsStringAsync().Result;
        var desContent = JsonSerializer.Deserialize<AuthResponse>(content, options);
        var token = desContent.Token;

        _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        
        InitializeTestUserByEmail();
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        _client.Dispose();
        _context.Dispose();
    }

    private async void InitializeTestUserByEmail()
    {
        User testUser = await CheckTestUser();
        if (testUser == null)
        { 
            await RegisterTestUser("testemail@testemail.com", "testuser", "testpassword");
            User newTestUser = await CheckTestUser();
            _userId = newTestUser.Id;
        }
        else
        {
            _userId = testUser.Id;
        }
    }

    private async Task<User> CheckTestUser()
    {
        try
        {
            var response = await _client.GetAsync("/VisualizeDo/GetUserByEmail?email=testemail@testemail.com");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            User user = JsonSerializer.Deserialize<User>(content, options);
            return user;
        }
        catch (HttpRequestException ex)
        {
            return new User();
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

    [Test]
    public void Test1()
    {
    }
}