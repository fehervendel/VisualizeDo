using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisualizeDo.Models;

public class Card
{
    [Key]
    public int Id { get; init; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public string? Size { get; set; }
    public int ListId { get; set; }
    [JsonIgnore]
    public List List { get; set; }
}