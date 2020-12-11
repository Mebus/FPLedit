using System;
using System.Collections.Generic;
using System.Linq;

namespace FPLedit.Shared
{
    /// <summary>
    /// Train name calculator with a specified name for each train.
    /// </summary>
    [Templating.TemplateSafe]
    public class SpecialTrainNameCalculator : ITrainLinkNameCalculator
    {
        internal const string PREFIX = "Special";

        public string[] Names { get; private set; }

        /// <inheritdoc />
        public void Deserialize(IEnumerable<string> parts)
        {
            if (parts.Count() < 2)
                throw new ArgumentException("parts is not long enough!");
            Names = parts.Skip(1).ToArray();
        }

        /// <inheritdoc />
        public IEnumerable<string> Serialize() => new[] { PREFIX }.Concat(Names);

        /// <inheritdoc />
        public string GetTrainName(int countingIndex) => Names[countingIndex];

        /// <summary>
        /// Initialize a new empty instance.
        /// </summary>
        public SpecialTrainNameCalculator()
        {
            Names = Array.Empty<string>();
        }

        /// <summary>
        /// Create a new instance, providing data.
        /// </summary>
        public SpecialTrainNameCalculator(string[] names)
        {
            Names = names;
        }
    }
}