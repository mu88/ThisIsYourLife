using FluentAssertions;
using NUnit.Framework;
using Persistence;

namespace Tests.Unit.Persistence;

[TestFixture]
[Category("Unit")]
public class NoImageExceptionTests
{
    [Test]
    public void DefaultConstructor()
    {
        var exception = new NoImageException();

        exception.Message.Should().NotBeNullOrEmpty();
        exception.InnerException.Should().BeNull();
    }

    [Test]
    public void MessageConstructor()
    {
        const string message = "test message";

        var exception = new NoImageException(message);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().BeNull();
    }

    [Test]
    public void MessageAndInnerExceptionConstructor()
    {
        const string message = "test message";
        var innerException = new InvalidOperationException("inner");

        var exception = new NoImageException(message, innerException);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(innerException);
    }
}
