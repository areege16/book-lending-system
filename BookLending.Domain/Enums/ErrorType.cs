namespace BookLending.Domain.Enums
{
    public enum ErrorType
    {
        None = 0,
        UnknownError = 1,

        ValidationFailed = 100,

        Unauthorized = 200,
        Forbidden = 201,
        InvalidCredentials = 203,

        Conflict = 300,
        AlreadyExists = 301,
        BadRequest = 400,
        NotFound = 404,

        InternalServerError = 500,
        UnexpectedError = 502
    }
}