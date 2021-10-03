using System;
using System.IO;

namespace DTO.LifePoint
{
    public record LifePointToCreate(DateOnly Date,
                                    string Caption,
                                    string Description,
                                    double Latitude,
                                    double Longitude,
                                    Guid CreatedBy,
                                    Stream? ImageStream = null);
}