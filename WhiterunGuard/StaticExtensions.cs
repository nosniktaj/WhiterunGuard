namespace WhiterunGuard
{
    public static class StaticExtensions
    {
        public static DateTime TimeAccurateToMinutes(this DateTime dt) => new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
    }
}