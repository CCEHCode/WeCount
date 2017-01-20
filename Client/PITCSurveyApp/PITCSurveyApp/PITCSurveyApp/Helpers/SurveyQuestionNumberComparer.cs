using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PITCSurveyApp.Helpers
{
    /// <summary>
    /// Implementation of <see cref="IComparer{T}"/> for survey question numbers. 
    /// </summary>
    class SurveyQuestionNumberComparer : IComparer<string>
    {
        private static readonly Regex s_regex = new Regex(@"(\d+)([a-z]*)(\d*)");

        private SurveyQuestionNumberComparer() { }

        /// <summary>
        /// The singleton instance of the comparer.
        /// </summary>
        public static readonly SurveyQuestionNumberComparer Instance = new SurveyQuestionNumberComparer();

        /// <summary>
        /// Compares two survey question numbers.
        /// </summary>
        /// <param name="x">The first survey question number.</param>
        /// <param name="y">The second survey question number.</param>
        /// <returns>
        /// <code>1</code> if the first value is greater than the second, 
        /// <code>-1</code> if the second value is greater than the first,
        /// otherwise <code>0</code>.
        /// </returns>
        public int Compare(string x, string y)
        {
            var xMatch = s_regex.Match(x);
            var yMatch = s_regex.Match(y);

            if (!xMatch.Success && !yMatch.Success)
            {
                return 0;
            }

            // A successful match is greater than an unsuccessful one
            if (xMatch.Success && !yMatch.Success)
            {
                return 1;
            }

            if (!xMatch.Success && yMatch.Success)
            {
                return -1;
            }

            if (xMatch.Groups.Count != 4 && yMatch.Groups.Count != 4)
            {
                return 0;
            }

            if (xMatch.Groups.Count == 4 && yMatch.Groups.Count != 4)
            {
                return 1;
            }

            if (xMatch.Groups.Count != 4 && yMatch.Groups.Count == 4)
            {
                return -1;
            }

            // Guaranteed to parse because of regex pattern
            var xMajorNumber = int.Parse(xMatch.Groups[1].Value);
            var yMajorNumber = int.Parse(yMatch.Groups[1].Value);
            var majorNumberComparison = xMajorNumber.CompareTo(yMajorNumber);
            if (majorNumberComparison != 0)
            {
                return majorNumberComparison;
            }

            var xMinorLetterString = xMatch.Groups[2].Value;
            var yMinorLetterString = yMatch.Groups[2].Value;
            if (string.IsNullOrEmpty(xMinorLetterString) && string.IsNullOrEmpty(yMinorLetterString))
            {
                return 0;
            }

            // A question with a minor letter is greater than one without
            if (!string.IsNullOrEmpty(xMinorLetterString) && string.IsNullOrEmpty(yMinorLetterString))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(xMinorLetterString) && !string.IsNullOrEmpty(yMinorLetterString))
            {
                return -1;
            }

            var minorLetterComparison = string.CompareOrdinal(xMinorLetterString, yMinorLetterString);
            if (minorLetterComparison != 0)
            {
                return minorLetterComparison;
            }

            var xMinorNumberString = xMatch.Groups[3].Value;
            var yMinorNumberString = yMatch.Groups[3].Value;
            if (string.IsNullOrEmpty(xMinorNumberString) && string.IsNullOrEmpty(yMinorNumberString))
            {
                return 0;
            }

            // A question with a minor letter is greater than one without
            if (!string.IsNullOrEmpty(xMinorNumberString) && string.IsNullOrEmpty(yMinorNumberString))
            {
                return 1;
            }

            if (string.IsNullOrEmpty(xMinorNumberString) && !string.IsNullOrEmpty(yMinorNumberString))
            {
                return -1;
            }

            // Guaranteed to parse because of regex pattern
            var xMinorNumber = int.Parse(xMinorNumberString);
            var yMinorNumber = int.Parse(yMinorNumberString);
            return xMinorNumber.CompareTo(yMinorNumber);
        }
    }
}
