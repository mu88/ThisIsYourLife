using System.ComponentModel.DataAnnotations;
using WebApp.Shared;

namespace WebApp.Models;

public class UserModel
{
    [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Main))]
    [StringLength(30, MinimumLength = 1, ErrorMessageResourceName = "LengthError", ErrorMessageResourceType = typeof(Main))]
    public string Name { get; set; } = string.Empty;
}