namespace BookLending.Domain.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Reader = "Reader";

        public static readonly string[] AllowedRoles = { Admin, Reader };
    }
}