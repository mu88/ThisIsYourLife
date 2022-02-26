using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Shared;

// TODO mu88: Nullability?
public class NewLifePointModel
{
    [Required]
    public string Caption { get; set; }

    public string Description { get; set; }

    [Required]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);
}