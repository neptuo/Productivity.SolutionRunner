using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neptuo.Productivity.SolutionRunner.Services.Statistics
{
    /// <summary>
    /// A month of a year.
    /// </summary>
    public class Month
    {
        /// <summary>
        /// Gets a month number.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Gets a year.
        /// </summary>
        public int Year { get; private set; }

        public Month(int year, int month)
        {
            Ensure.Positive(year, "year");
            Ensure.Positive(month, "month");

            if (month > 12)
                throw Ensure.Exception.ArgumentOutOfRange("value", "A month must be between 1 and 12.");

            Value = month;
            Year = year;
        }

        public override bool Equals(object obj)
        {
            Month other = obj as Month;
            if (other == null)
                return false;

            return Value == other.Value && Year == other.Year;
        }

        public override int GetHashCode()
        {
            int hash = 37;
            hash += 11 * Value;
            hash += 11 * Year;
            return hash;
        }

        public override string ToString()
        {
            if (Year == DateTime.Now.Year)
                return ToShortString();

            return $"{ToShortString()} {Year}";
        }

        public string ToShortString()
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames[Value - 1];
        }

        /// <summary>
        /// Creates an instance of <see cref="Month"/> from <paramref name="dateTime"/>.
        /// </summary>
        /// <param name="dateTime">A source date and time.</param>
        public static implicit operator Month(DateTime dateTime)
        {
            return new Month(dateTime.Year, dateTime.Month);
        }

        public static bool operator ==(Month a, Month b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Month a, Month b)
        {
            return !(a == b);
        }

        public static bool operator >(Month a, Month b)
        {
            if ((object)a == null)
                return false;

            if ((object)b == null)
                return true;

            if (a.Year > b.Year)
                return true;

            if (a.Year == b.Year)
                return a.Value > b.Value;

            return false;
        }

        public static bool operator <(Month a, Month b)
        {
            if ((object)a == null)
                return true;

            if ((object)b == null)
                return false;

            if (a.Year < b.Year)
                return true;

            if (a.Year == b.Year)
                return a.Value < b.Value;

            return false;
        }
    }
}
