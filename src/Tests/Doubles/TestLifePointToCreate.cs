using System;
using System.IO;
using DTO.LifePoint;
using Entities;

namespace Tests.Doubles;

public static class TestLifePointToCreate
{
    public static LifePointToCreate Create(Person? person = null, Stream? imageStream = null)
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var caption = "caption";
        var description = "description";
        var latitude = 12.6;
        var longitude = 34.3;
        var createdBy = person?.Id ?? Guid.NewGuid();
        var lifePointToCreate = new LifePointToCreate(date,
                                                      caption,
                                                      description,
                                                      latitude,
                                                      longitude,
                                                      createdBy,
                                                      imageStream);
        return lifePointToCreate;
    }
}