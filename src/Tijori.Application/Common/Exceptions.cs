namespace Tijori.Application.Common;

public class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}

public class ValidationAppException : AppException
{
    public IReadOnlyList<string> Errors { get; }

    public ValidationAppException(IEnumerable<string> errors)
        : base("Validation failed.")
    {
        Errors = errors.ToList();
    }
}

public class NotFoundAppException : AppException
{
    public NotFoundAppException(string message) : base(message)
    {
    }
}

public class UnauthorizedAppException : AppException
{
    public UnauthorizedAppException(string message) : base(message)
    {
    }
}

public class ConflictAppException : AppException
{
    public ConflictAppException(string message) : base(message)
    {
    }
}
