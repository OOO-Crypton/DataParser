namespace DataParserDatabase.DatabaseSettings
{
    public static class Settings
    {
        public static string Server { get; private set; } = "localhost";
        public static string Database { get; private set; } = "CoinsDatabase";
        public static string Port { get; private set; } = "5432";
        public static string User { get; private set; } = "postgres";
        public static string Password { get; private set; } = "1234";
    }
}
