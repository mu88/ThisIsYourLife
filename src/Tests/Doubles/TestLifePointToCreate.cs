using System;
using DTO;
using Entities;

namespace Tests.Doubles
{
    public static class TestLifePointToCreate
    {
        public static LifePointToCreate Create(Person person)
        {
            var date = DateTime.Now;
            string caption = "caption";
            string description = "description";
            var latitude = 12.6;
            var longitude = 34.3;
            var createdBy = person.Id;
            var lifePointToCreate = new LifePointToCreate(date,
                                                          caption,
                                                          description,
                                                          latitude,
                                                          longitude,
                                                          createdBy);
            return lifePointToCreate;
        }
    }
}