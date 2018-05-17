namespace LogCast
{
    public static class CorrelationIdHeader
    {
        public const string Preferred = "Correlation-Id";
        public static readonly string[] Accepted = { Preferred, "X-Correlation-Id" };
    }
}