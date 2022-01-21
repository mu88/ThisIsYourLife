using Moq.AutoMock;
using Tests.Doubles;

namespace Tests.UnitTests;

public class CustomAutoMocker : AutoMocker
{
    /// <inheritdoc />
    public CustomAutoMocker() => Use(TestMapper.Create());
}