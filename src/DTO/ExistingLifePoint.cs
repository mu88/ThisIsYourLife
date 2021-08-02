using System;

namespace DTO
{
    public class ExistingLifePoint
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string CreatedBy { get; set; }
    }
}