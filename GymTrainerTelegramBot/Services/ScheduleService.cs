using GymTrainerTelegramBot.Abstract;
using GymTrainerTelegramBot.Models;

namespace GymTrainerTelegramBot.Services;

public class ScheduleService(ApplicationContext context): IScheduleService
{
    public void CreateWorkoutsIfNotExists()
    {
        var beginningDate = DateOnly.FromDateTime(DateTime.Now);
        var endDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)); // TODO: Конфигуркция

        var beginningTimePoint = new TimePoint(9, 0); // TODO: Конфигуркция
        var endTimePoint = new TimePoint(20, 0); // TODO: Конфигуркция

        var lunchTimeRange = new TimeRange(new TimePoint(15, 0), new TimePoint(16, 0)); // TODO: Конфигуркция

        var timePoints = TimePoint.Enumerate(beginningTimePoint, endTimePoint, 1, 0);

        foreach (var timePoint in timePoints)
        {
            if (lunchTimeRange.Contains(timePoint))
            {
                continue;
            }

            Console.WriteLine(timePoint.Hour + ":" + timePoint.Minute);
        }
    }

    private class TimePoint
    {
        private int _hour;

        public int Hour
        {
            get => _hour;
            set
            {
                if (value is < 0 or > 23) 
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _hour = value;
            }
        }

        private int _minute;

        public int Minute
        {
            get => _minute;
            set
            {
                if (value is < 0 or > 59)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _minute = value;
            }
        }

        public TimePoint(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        public static IEnumerable<TimePoint> Enumerate(TimePoint start, TimePoint end, int HourStep, int MinuteStep)
        {
            var result = new List<TimePoint>();

            var timePoint = new TimePoint(start.Hour, start.Minute);

            while (timePoint < end)
            {
                result.Add(timePoint);
                timePoint = new TimePoint(timePoint.Hour + HourStep, timePoint.Minute + MinuteStep);
            }

            return result;
        }


        public static bool operator >=(TimePoint left, TimePoint right)
        {
            if (left.Hour > right.Hour)
            {
                return true;
            }
            else if (left.Hour == right.Hour)
            {
                return left.Minute >= right.Minute;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(TimePoint left, TimePoint right)
        {
            if (left.Hour < right.Hour)
            {
                return true;
            }
            else if (left.Hour == right.Hour)
            {
                return left.Minute <= right.Minute;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(TimePoint left, TimePoint right)
        {
            if (left.Hour > right.Hour)
            {
                return true;
            }
            else if (left.Hour == right.Hour)
            {
                return left.Minute > right.Minute;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <(TimePoint left, TimePoint right)
        {
            if (left.Hour < right.Hour)
            {
                return true;
            }
            else if (left.Hour == right.Hour)
            {
                return left.Minute < right.Minute;
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(TimePoint left, TimePoint right)
        {
            return left.Hour == right.Hour && left.Minute == right.Minute;
        }

        public static bool operator !=(TimePoint left, TimePoint right)
        {
            return left.Hour != right.Hour || left.Minute != right.Minute;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is not TimePoint otherTimePoint)
            {
                return false;
            }

            return Hour == otherTimePoint.Hour && Minute == otherTimePoint.Minute;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hour, Minute);
        }
    }

    private class TimeRange
    {
        public TimePoint StartTimePoint { get; set; }

        public TimePoint EndTimePoint { get; set; }

        public TimeRange(TimePoint startTimePoint, TimePoint endTimePoint)
        {
            if (startTimePoint > endTimePoint)
            {
                throw new ArgumentException("Начало временного отрезка не может быть больше конца");
            }
            StartTimePoint = startTimePoint;
            EndTimePoint = endTimePoint;
        }

        public bool Contains(TimePoint timePoint)
        {
            if (timePoint >= StartTimePoint && timePoint < EndTimePoint)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}