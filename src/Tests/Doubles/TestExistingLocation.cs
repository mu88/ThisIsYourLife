using DTO.Location;

namespace Tests.Doubles;

public static class TestExistingLocation
{
    public static ExistingLocation Create() => new(Random.Shared.Next(-90, 90), Random.Shared.Next(-90, 90), Guid.NewGuid());
}
