namespace DataAccessLayer.Errors
{
    public static class RegexErrors
    {
        public const string PhonePattern = "Invalid Egy Number Format";
        public const string PasswordPattern = "Password Must have minimum 6 characters with At least 1 uppercase and 1 lowercase English letter and 1 digit and 1 special character.";
        public const string PasswordMisMatch = "The password and confirm password does not match";
    }
}
