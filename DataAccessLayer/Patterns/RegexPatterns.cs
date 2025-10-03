namespace DataAccessLayer.Patterns
{
    public static class RegexPatterns
    {
        public const string PasswordPattern = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{6,}$";

        public const string EmailPattern = "^\\S+@\\S+\\.\\S+$";

        public const string EgyPhonePattern = "^01[0125][0-9]{8}$";

        public const string NumericsonlyPattern = "^\\d+$";

        public const string EnglishLettersOnlyPattern = "^[a-zA-Z]+$";
        public const string EnglishLettersOnlyWithSpacePattern = "^[a-zA-Z ]+$";

        public const string EnglishLettersandDotPattern = "^[a-zA-Z. ]+$";

        public const string EnglishLettersAndNumbersOnlyPattern = "^[a-zA-Z0-9]+$";

        public const string EgyNationalNumber = "^([23]{1})([0-9]{13})$";

        public const string EgyPostalCode = "^[1-9]{1}[0-9]{4}$";

    }
}
