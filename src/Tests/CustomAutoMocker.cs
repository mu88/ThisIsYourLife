using Moq.AutoMock;

namespace Tests
{
    public class CustomAutoMocker : AutoMocker
    {
        /// <inheritdoc />
        public CustomAutoMocker() => Use(TestMapper.Create());
    }
}