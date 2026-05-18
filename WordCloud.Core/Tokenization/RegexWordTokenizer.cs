using System.Text.RegularExpressions;

namespace WordCloud.Core.Tokenization;

public sealed class RegexWordTokenizer : IWordTokenizer
{
    private static readonly Regex WordRegex = new(
        @"[\p{L}\p{M}\p{N}]+(?:['’-][\p{L}\p{M}\p{N}]+)*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public IEnumerable<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        foreach (Match match in WordRegex.Matches(text))
        {
            yield return match.Value;
        }
    }
}