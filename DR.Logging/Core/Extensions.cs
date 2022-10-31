using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DR.Logging.Core
{
    internal static class Extensions
    {
        /// <summary>
        /// Add all numbers in a string that appear in a row to a integer list.
        /// If other characters appear between numbers add the next set to a new index.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static List<int> SplitNumbersInString(this string s)
        {
            s = s.Trim();
            List<int> result = new List<int>();
            try
            {
                List<int> numbers = new List<int>();

                for (int count = 0; count < s.Length; count++)
                {
                    char c = s[count];
                    if ((count + 1) == s.Length)
                    {
                        if (char.IsNumber(c))
                            numbers.Add(int.Parse(c.ToString()));

                        result.Add(int.Parse(IntToString(numbers)));
                    }
                    else if (char.IsNumber(c))
                    {
                        numbers.Add(int.Parse(c.ToString()));
                    }
                    else if (numbers.Count > 0)
                    {
                        result.Add(int.Parse(IntToString(numbers)));
                        numbers.Clear();
                    }
                }
            }
            catch { }
            return result;
        }

        /// <summary>
        /// Convert a list of integers to a string
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        private static string IntToString(List<int> numbers)
        {
            string result = string.Empty;
            numbers.ForEach(x => result += x.ToString());
            return result;
        }
    }
}
