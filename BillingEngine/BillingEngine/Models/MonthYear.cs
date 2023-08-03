using System;

namespace BillingEngine.Models
{
    public class MonthYear
    {
        public int Month { get; }
        public int Year { get; }

        public bool IsLesserThan(DateTime dateTime)
        {
            if (Year < dateTime.Year)
            {
                return true;
            }

            if (Year > dateTime.Year)
            {
                return false;
            }

            return Month < dateTime.Month;
        }
    }
}