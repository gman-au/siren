using System;
using System.Linq;

namespace Siren.Infrastructure.AssemblyLoad.Extensions
{
    internal static class StringEx
    {
        public static Tuple<string, string> SplitNamespace(this string fullName)
        {
            var items = fullName.Split('.');

            if (items.Length > 1)
            {
                return new Tuple<string, string>(
                    string.Join('.', items.Where(o => o != items.Last())),
                    items.Last()
                );
            }

            return new Tuple<string, string>(
                fullName,
                fullName
            );
        }

        public static string ToEscaped(this string value)
        {
            value =
                value
                    .Replace(
                        ".",
                        "_"
                    )
                    .Replace(
                        "(",
                        ""
                    )
                    .Replace(
                        ")",
                        ""
                    )
                    .Replace(
                        ",",
                        "_"
                    );

            return value;
        }
    }
}