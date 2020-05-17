﻿using System;

namespace PlatoCore.Abstractions.Extensions
{

    public static class DateTimeExtensions
    {

        public static DateTime ToDateIfNull(this DateTime? input)
        {
            return input ?? System.DateTime.UtcNow;
        }

        public static string ToPrettyDate(this DateTime? d)
        {
            return d == null ? string.Empty : ToPrettyDate((DateTime) d);
        }

        public static string ToPrettyDate(
            this DateTime d, 
            string fallbackFormat = "G")
        {

            var s = DateTime.UtcNow.Subtract(d);
            
            var dayDiff = (int) s.TotalDays;
            var weekDiff = (int) Math.Floor((double) dayDiff / 7);
            var monthDiff = (int) Math.Floor((double) weekDiff / 4);
  
            if (dayDiff < 0 || monthDiff > 3)
            {
                return d.ToString(fallbackFormat);
            }

            // Months / Weeks / Days

            if (monthDiff > 0)
            {
                return monthDiff == 1
                    ? $"{monthDiff} month ago"
                    : $"{monthDiff} months ago";
            }

            if (weekDiff > 0)
            {
                return weekDiff == 1
                    ? $"{weekDiff} week ago"
                    : $"{weekDiff} weeks ago";
            }
            
            if (dayDiff == 1)
            {
                return "yesterday";
            }

            if (dayDiff > 0 && dayDiff < 7)
            {
                return $"{dayDiff} days ago";
            }

            // Seconds / Minutes / Hours

            var secDiff = (int)s.TotalSeconds;

            if (secDiff < 60)
            {
                return "just now";
            }

            if (secDiff < 120)
            {
                return "1 minute ago";
            }

            if (secDiff < 3600)
            {
                return $"{Math.Floor((double) secDiff / 60)} minutes ago";
            }

            if (secDiff < 7200)
            {
                return "1 hour ago";
            }

            if (secDiff < 86400)
            {
                return $"{Math.Floor((double) secDiff / 3600)} hours ago";
            }
            
            return null;

        }

        public static int DayDifference(this DateTime input, DateTime date)
        {       
            return date.Subtract(input).Days;
        }

        public static string ToSortableDateTimePattern(this DateTime input, System.Globalization.CultureInfo culture = null)
        {
            if (culture == null)
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.invariantculture?view=netcore-3.1
                culture = System.Globalization.CultureInfo.InvariantCulture;
            }
            return input.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.SortableDateTimePattern);
        }

        public static DateTime Floor(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 0, 0, 0);
        }

        public static DateTime Ceil(this DateTime d)
        {
            return new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);
        }

    }

}
