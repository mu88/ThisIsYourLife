using System;
using System.Diagnostics.CodeAnalysis;

// EF Core needs default constructor
#pragma warning disable 8618

namespace Entities;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "EF Core uses them")]
public class LifePoint
{
    // ReSharper disable once UnusedMember.Global - EF Core needs default constructor
    public LifePoint()
    {
    }

    public LifePoint(DateOnly date,
                     string caption,
                     string description,
                     double latitude,
                     double longitude,
                     Person createdBy,
                     Guid? imageId = null)
    {
        Id = Guid.NewGuid();
        Date = date;
        Caption = caption;
        Description = description;
        Latitude = latitude;
        Longitude = longitude;
        CreatedBy = createdBy;
        ImageId = imageId;
    }

    public Guid Id { get; private set; }

    public DateOnly Date { get; private set; }

    public string Caption { get; private set; }

    public string Description { get; private set; }

    public double Latitude { get; private set; }

    public double Longitude { get; private set; }

    public virtual Person CreatedBy { get; private set; }

    public Guid? ImageId { get; private set; }
}