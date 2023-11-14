using System.ComponentModel.DataAnnotations;

namespace VisualizeDo.Models;

public class Card
{
    [Key]
    public int Id { get; init; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public string? Size { get; set; }
}