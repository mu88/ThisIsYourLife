using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared;

public class UserModel
{
    [Required]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 30 characters long.")]
    public string Name { get; set; } = string.Empty;
}