using System.Globalization;

namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - dob.Year;

            if (dob > today.AddYears(-age)) age--;

            return age;
        }

        public static DateOnly? ConvertStringToDateOnly(this string dob) 
        {
            if (string.IsNullOrEmpty(dob)) return null;

            return DateOnly.ParseExact(dob, "yyyy-mm-dd", CultureInfo.InvariantCulture);
        }
    }
}