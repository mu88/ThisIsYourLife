using System;
using DTO.Person;

namespace DTO.LifePoint;

public record ExistingLifePoint(Guid Id,
                                DateOnly Date,
                                string Caption,
                                string Description,
                                double Latitude,
                                double Longitude,
                                ExistingPerson CreatedBy,
                                Guid? ImageId);