using System;
using Entities;

namespace Tests.Doubles
{
    public static class TestLifePoint
    {
        public static LifePoint Create(Person? createdBy = null)
        {
            createdBy ??= new Person("Oscar");
            var random = new Random();
            return new LifePoint(DateOnly.FromDateTime(DateTime.Now),
                                 $"Caption {random.Next()}",
                                 $"Description {random.Next()}",
                                 random.Next(-90, 90),
                                 random.Next(-180, 180),
                                 createdBy);
        }
    }
}