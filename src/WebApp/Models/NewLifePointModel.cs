using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class NewLifePointModel
{
    [Required]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Must be between 1 and 30 characters long.")]
    public string Caption { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Must be between 1 and 200 characters long.")]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}