using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace TempDynamic
{
    public struct Range
    {
        public int Difference => this.Maximum - this.Minimum;
        public int Minimum;
        public int Maximum;

        public Range(int min, int max)
        {
            this.Minimum = min;
            this.Maximum = max;
            if (this.Difference < 0)
                throw new ValidationMetadataException(
                    string.Format(
                        "{0} is fewer than {1}.",
                        nameof(Maximum),
                        nameof(Minimum)
                    )
                );
        }

        public static implicit operator Range((int, int) tuple)
        {
            return new Range(tuple.Item1, tuple.Item2);
        }
        public static explicit operator (int, int)(Range range)
        {
            return (range.Minimum, range.Maximum);
        }

        public static Range? ToRange((int, int)? tuple)
        {
            return tuple.HasValue
                ? new Range(tuple.Value.Item1, tuple.Value.Item2)
                : (Range?)null;
        }
        internal static Range? TryMakeRange(ValidateCountAttribute attribute)
        {
            return attribute != null
                ? new Range(attribute.MinLength, attribute.MaxLength)
                : (Range?)null;
        }
        internal static Range? TryMakeRange(ValidateLengthAttribute attribute)
        {
            return attribute != null
                ? new Range(attribute.MinLength, attribute.MaxLength)
                : (Range?)null;
        }
    }
}
