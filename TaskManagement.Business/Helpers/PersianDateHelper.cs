using System.Globalization;

namespace TaskManagement.Business.Helpers;

public static class PersianDateHelper
{
    private static readonly PersianCalendar PersianCalendar = new();

    private static readonly TimeZoneInfo TehranTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows()
                ? "Iran Standard Time"
                : "Asia/Tehran");

    public static string ToPersianDate(DateTime utcDate)
    {
        var tehranDate = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDate, DateTimeKind.Utc),
            TehranTimeZone);

        return $"{PersianCalendar.GetYear(tehranDate):0000}/" +
               $"{PersianCalendar.GetMonth(tehranDate):00}/" +
               $"{PersianCalendar.GetDayOfMonth(tehranDate):00}";
    }

    public static string ToPersianDateTime(DateTime utcDate)
    {
        var tehranDate = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.SpecifyKind(utcDate, DateTimeKind.Utc),
            TehranTimeZone);

        return $"{PersianCalendar.GetYear(tehranDate):0000}/" +
               $"{PersianCalendar.GetMonth(tehranDate):00}/" +
               $"{PersianCalendar.GetDayOfMonth(tehranDate):00} " +
               $"{tehranDate:HH:mm}";
    }
}