using System;

namespace Web.Exceptions
{
    public class UserVisibleException : Exception
    {
        public string UserVisibleMessage { get; }

        public UserVisibleException(string userVisibleMessage)
            : base(userVisibleMessage)
        {
            UserVisibleMessage = userVisibleMessage;
        }

        public UserVisibleException(string message, string userVisibleMessage)
            : base(message)
        {
            UserVisibleMessage = userVisibleMessage;
        }

        public UserVisibleException(
            string message,
            string userVisibleMessage,
            Exception innerException
        )
            : base(message, innerException)
        {
            UserVisibleMessage = userVisibleMessage;
        }
    }
}
