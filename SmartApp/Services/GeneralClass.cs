namespace SmartApp.Services
{
    public static class GeneralClass
    {
        public static int GetNumericFromText(string text, int startIndex, int endIndex)
        {
            if (endIndex > text.Length)
                endIndex = text.Length;

            if (startIndex < 0)
            {
                throw new ArgumentException("start index is negative");
            }

            if (endIndex < startIndex)
            {
                throw new ArgumentException("end index less than start index");
            }

            if (text.Length == 0)
                return 0;

            int result;
            Int32.TryParse(text.Substring(startIndex, endIndex - startIndex + 1).Trim(), out result);
            return result;
        }
    }
}
