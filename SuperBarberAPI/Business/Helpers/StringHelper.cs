namespace Business.Helpers
{
    public static class StringHelper
    {
        public static string RandomStringGenerator(int length)
        {
            Random random = new ();
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890abcdefghijklmnopqrstuvwxyz_";

            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
