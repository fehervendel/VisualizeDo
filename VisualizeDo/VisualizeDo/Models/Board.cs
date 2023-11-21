using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace VisualizeDo.Models;

public class Board
{
    [Key]
    public int Id { get; init; }
    public string? Name { get; set; }
    public List<List>? Lists { get; set; }
    public int UserId { get; set; }
    [JsonIgnore]
    public User User { get; set; }
}