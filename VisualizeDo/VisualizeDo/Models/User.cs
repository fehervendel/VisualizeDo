using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace VisualizeDo.Models;

public class User
{
    [Key]
    public int Id { get; init; }
    public string Name { get; set; }
    public string EmailAddress { get; set; }
    public string IdentityUserId { get; set; }
    public IdentityUser IdentityUser { get; set; }
}