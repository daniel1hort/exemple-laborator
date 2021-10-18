using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L01.Extensions
{
    public record StringMultiplication(int Value)
    {
        public static implicit operator StringMultiplication(int value)
            => new StringMultiplication(value);
        public static implicit operator int(StringMultiplication value)
            => value.Value;
        public static string operator *(string @base, StringMultiplication count)
            => Enumerable.Repeat(@base, count.Value).Aggregate((a, b) => a + b);
        public static string operator *(StringMultiplication count, string @base)
            => Enumerable.Repeat(@base, count.Value).Aggregate((a, b) => a + b);
    }
}
