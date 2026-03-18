namespace BookLending.Domain.Enums
{
    public enum ErrorType
    {
        None = 0,
        UnknownError = 1,

        ValidationFailed = 100,
        InvalidInput = 101,
        MissingRequiredField = 102,
        InvalidEmailFormat = 103,
        PasswordMismatch = 104,
        InvalidRole = 105,

        Unauthorized = 200,
        Forbidden = 201,
        TokenExpired = 202,
        InvalidCredentials = 203,

        Conflict = 300,
        AlreadyExists = 301,
        NotFound = 302,
        DuplicateEmail = 303,
        DuplicateUserName = 304,

        DependencyFailure = 400,
        ThirdPartyError = 401,
        Timeout = 402,
        DatabaseError = 403,

        InternalServerError = 500,
        OperationFailed = 501,

        UnexpectedError = 502
    }
}