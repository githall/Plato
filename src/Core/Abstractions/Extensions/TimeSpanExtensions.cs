using System;
using System.Collections.Generic;

namespace PlatoCore.Abstractions.Extensions
{

    public static class TimeSpanExtensions
    {

        public static TimeSpan Seconds(this IEnumerable<TimeSpan> timeSpans)
        {

            var seconds = 0;
            foreach (var timeSpan in timeSpans)
            {
                seconds = ((int)timeSpan.Seconds) + seconds;
            }

            return TimeSpan.FromSeconds(seconds);

        }

        public static TimeSpan TotalSeconds(this IEnumerable<TimeSpan> timeSpans)
        {

            var totalSeconds = 0;
            foreach (var timeSpan in timeSpans)
            {
                totalSeconds = ((int)timeSpan.TotalSeconds) + totalSeconds;
            }

            return TimeSpan.FromSeconds(totalSeconds);

        }

        public static TimeSpan Milliseconds(this IEnumerable<TimeSpan> timeSpans)
        {

            var milliseconds = 0;
            foreach (var timeSpan in timeSpans)
            {
                milliseconds = ((int)timeSpan.Milliseconds) + milliseconds;
            }

            return TimeSpan.FromMilliseconds(milliseconds);

        }

        public static TimeSpan MilliseconTotalMillisecondsds(this IEnumerable<TimeSpan> timeSpans)
        {

            var totalMilliseconds = 0;
            foreach (var timeSpan in timeSpans)
            {
                totalMilliseconds = ((int)timeSpan.TotalMilliseconds) + totalMilliseconds;
            }

            return TimeSpan.FromMilliseconds(totalMilliseconds);

        }

    }

}