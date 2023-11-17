using FluentValidation.Results;
using Web.Exceptions;

namespace Web.Infrastructure.Exceptions
{
    public class UserVisibleValidationException : UserVisibleException
    {
        public UserVisibleValidationException(string userVisibleMessage)
            : base(userVisibleMessage) { }

        public UserVisibleValidationException(string message, string userVisibleMessage)
            : base(message, userVisibleMessage) { }

        public UserVisibleValidationException(
            string message,
            string userVisibleMessage,
            Exception innerException
        )
            : base(message, userVisibleMessage, innerException) { }

        public UserVisibleValidationException(IEnumerable<ValidationFailure> validationFailures)
            : base(GetValidationExceptionMessage(validationFailures)) { }

        private static string GetValidationExceptionMessage(
            IEnumerable<ValidationFailure> validationFailures
        ) =>
            "One or more validation errors occurred:"
            + string.Join(
                string.Empty,
                validationFailures.Select(
                    failure =>
                        $"{Environment.NewLine}- {failure.PropertyName}: {failure.ErrorMessage}"
                )
            );
    }
}
