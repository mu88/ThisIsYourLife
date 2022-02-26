using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class UserModel
{
    [Required]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Must be between 1 and 30 characters long.")]
    public string Name { get; set; } = string.Empty;
}