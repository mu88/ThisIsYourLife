namespace Persistence;

public class NoImageException : Exception
{
    public NoImageException()
    {
    }

    public NoImageException(string message)
        : base(message)
    {
    }

    public NoImageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
