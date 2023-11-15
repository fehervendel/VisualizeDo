using System.ComponentModel.DataAnnotations;

namespace VisualizeDo.Contracts;

public record RegistrationRequest(
    [Required]string Email, 
    [Required]string Username, 
    [Required]string Password
);