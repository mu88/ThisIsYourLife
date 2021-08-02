using System;

// EF Core needs default constructor
#pragma warning disable 8618

namespace Entities
{
    public class LifePoint
    {
        public LifePoint()
        {
        }

        public LifePoint(DateTime date,
                         string caption,
                         string description,
                         double latitude,
                         double longitude,
                         Person createdBy)
        {
            Id = Guid.NewGuid();
            Date = date;
            Caption = caption;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            CreatedBy = createdBy;
        }

        public Guid Id { get; }

        public DateTime Date { get; }

        public string Caption { get; }

        public string Description { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public Person CreatedBy { get; }
    }
}