using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace VisualizeDo.Models;

public class List
{
    [Key]
    public int Id { get; init; }
    public string? Name { get; set; }
    public List<Card>? Cards { get; set; }
    public int BoardId { get; set; }
    [JsonIgnore]
    public Board Board { get; set; }
}