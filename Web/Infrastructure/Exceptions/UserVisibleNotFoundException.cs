namespace Web.Infrastructure.Exceptions;

public class UserVisibleNotFoundException : UserVisibleException
{
    public UserVisibleNotFoundException(string userVisibleMessage)
        : base(userVisibleMessage) { }

    public UserVisibleNotFoundException(string message, string userVisibleMessage)
        : base(message, userVisibleMessage) { }
}
