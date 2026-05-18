namespace WordCloud.Core.Options;

public sealed class WordCloudOptions
{
    public bool IgnoreCase { get; init; } = true;

    public int MinWordLength { get; init; } = 1;

    public WordSortMode SortMode { get; init; } =
        WordSortMode.FrequencyDescendingThenAlphabetical;

    public ISet<string> StopWords { get; init; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static WordCloudOptions Default => new();
}