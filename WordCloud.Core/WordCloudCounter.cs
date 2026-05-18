using WordCloud.Core.Models;
using WordCloud.Core.Options;
using WordCloud.Core.Tokenization;

namespace WordCloud.Core;

public sealed class WordCloudCounter
{
    private readonly IWordTokenizer _tokenizer;
    private readonly WordCloudOptions _options;

    public WordCloudCounter(
        IWordTokenizer? tokenizer = null,
        WordCloudOptions? options = null)
    {
        _tokenizer = tokenizer ?? new RegexWordTokenizer();
        _options = options ?? WordCloudOptions.Default;

        if (_options.MinWordLength < 1)
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "Minimum word length must be at least 1.");
    }

    public IReadOnlyList<WordFrequency> CountWords(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<WordFrequency>();

        var comparer = _options.IgnoreCase
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;

        var counts = new Dictionary<string, int>(comparer);

        foreach (var token in _tokenizer.Tokenize(text))
        {
            var word = Normalize(token);

            if (word.Length < _options.MinWordLength)
            {
                continue;
            }

            if (_options.StopWords.Contains(word))
            {
                continue;
            }

            counts[word] = counts.TryGetValue(word, out var currentCount)
                ? currentCount + 1
                : 1;
        }

        return Sort(counts)
            .Select(pair => new WordFrequency(pair.Key, pair.Value))
            .ToArray();
    }

    private string Normalize(string word)
    {
        return _options.IgnoreCase
            ? word.ToLowerInvariant()
            : word;
    }

    private IEnumerable<KeyValuePair<string, int>> Sort(
        Dictionary<string, int> counts)
    {
        return _options.SortMode switch
        {
            WordSortMode.Alphabetical => counts
                .OrderBy(pair => pair.Key, StringComparer.Ordinal),

            WordSortMode.FrequencyDescendingThenAlphabetical => counts
                .OrderByDescending(pair => pair.Value)
                .ThenBy(pair => pair.Key, StringComparer.Ordinal),

            _ => throw new ArgumentOutOfRangeException(
                nameof(_options.SortMode),
                "Unsupported sort mode.")
        };
    }
}