using System;

namespace DTO
{
    public class LifePointToCreate
    {
        public DateTime Date { get; }

        public string Caption { get; }

        public string Description { get; }

        public double Latitude { get; }

        public double Longitude { get; }

        public Guid CreatedBy { get; }

        public LifePointToCreate(DateTime date,
                                 string caption,
                                 string description,
                                 double latitude,
                                 double longitude,
                                 Guid createdBy)
        {
            Date = date;
            Caption = caption;
            Description = description;
            Latitude = latitude;
            Longitude = longitude;
            CreatedBy = createdBy;
        }
    }
}