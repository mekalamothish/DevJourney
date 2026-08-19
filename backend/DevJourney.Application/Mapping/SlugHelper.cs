using System.Text;
using System.Text.RegularExpressions;

namespace DevJourney.Application.Mapping
{
    internal static class SlugHelper
    {
        private static readonly Regex _invalidChars = new("[^a-z0-9-]", RegexOptions.Compiled);

        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var s = input.Trim().ToLowerInvariant();
            // replace spaces with dashes
            s = Regex.Replace(s, "\\s+", "-");
            // remove invalid chars
            s = _invalidChars.Replace(s, string.Empty);
            // collapse multiple dashes
            s = Regex.Replace(s, "-+", "-");
            return s.Trim('-');
        }
    }
}
