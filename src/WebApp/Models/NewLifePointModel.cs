using System.ComponentModel.DataAnnotations;
using WebApp.Shared;

namespace WebApp.Models;

public class NewLifePointModel
{
    [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Main))]
    [StringLength(50, MinimumLength = 1, ErrorMessageResourceName = "LengthError", ErrorMessageResourceType = typeof(Main))]
    public string Caption { get; set; } = string.Empty;

    [StringLength(500, ErrorMessageResourceName = "LengthError", ErrorMessageResourceType = typeof(Main))]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessageResourceName = "RequiredError", ErrorMessageResourceType = typeof(Main))]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}
