﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FPLedit.Standard
{
    internal class TimeNormalizer
    {
        private Regex verifyRegex;

        public TimeNormalizer()
        {
            //                        hh:mm, h:mm, h:m, hh:m, h:, :m           hhmm, hmm, mm
            verifyRegex = new Regex(@"^ (?<hr>\d{1,2})? : (?<min>\d{1,2})? $ | ^ (?<hr>\d{1,2})? (?<min>\d{2}) $",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
        }

        public string Normalize(string input)
        {
            var m = verifyRegex.Matches(input);
            if (m.Count == 1)
            {
                var hours = m[0].Groups["hr"].Value.PadLeft(2, '0');
                var minutes = m[0].Groups["min"].Value.PadLeft(2, '0');
                return hours + ":" + minutes;
            }
            return null;
        }
    }
}
