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

    private TestPrepare _testPrepare = new TestPrepare();

    [SetUp]
    public async Task Setup()
    {
        DotNetEnv.Env.Load();
        string connectionString = DotNetEnv.Env.GetString("CONNECTION_STRING");
        Environment.SetEnvironmentVariable("CONNECTION_STRING", "Server=localhost,1433;Database=VisualizeDo;User Id=sa;Password=Feher2023vendeL!;");
        
        _client = CreateClient();
        await _testPrepare.Prepare();
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
    
    
    [Test]
    public void Test1()
    {
    }
}