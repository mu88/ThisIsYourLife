using Entities;

namespace Tests.Doubles;

public static class TestLifePoint
{
    public static LifePoint Create(Person? createdBy = null, DateOnly? date = null, Guid? imageId = null)
    {
        createdBy ??= new Person("Oscar");
        date ??= DateOnly.FromDateTime(DateTime.Now);
        var random = new Random();
        return new LifePoint(date.Value,
            $"Caption {random.Next()}",
            $"Description {random.Next()}",
            random.Next(-90, 90),
            random.Next(-180, 180),
            createdBy,
            imageId);
    }
}
