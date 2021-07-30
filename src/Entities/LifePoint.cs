using System;

namespace Entities
{
    public class LifePoint
    {
        public LifePoint(Guid id,
                         DateTime date,
                         string caption,
                         string description,
                         double latitude,
                         double longitude,
                         Creator createdBy)
        {
            Id = id;
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

        public Creator CreatedBy { get; }
    }
}