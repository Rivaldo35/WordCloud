using FluentAssertions;
using WordCloud.Core;
using WordCloud.Core.Models;
using WordCloud.Core.Options;

namespace WordCloud.Tests;

public sealed class WordCloudCounterTests
{
    [Fact]
    public void CountWords_ShouldReturnEmptyResult_WhenTextIsNull()
    {
        // Arrange
        var counter = new WordCloudCounter();

        // Act
        var result = counter.CountWords(null);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CountWords_ShouldReturnEmptyResult_WhenTextIsEmpty()
    {
        // Arrange
        var counter = new WordCloudCounter();
        const string input = "";

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CountWords_ShouldCountWordsCaseInsensitiveByDefault()
    {
        // Arrange
        var counter = new WordCloudCounter();
        const string input = "Hello hello HELLO";

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().ContainSingle()
            .Which.Should().Be(new WordFrequency("hello", 3));
    }

    [Fact]
    public void CountWords_ShouldRespectCaseSensitivity_WhenIgnoreCaseIsFalse()
    {
        // Arrange
        var counter = new WordCloudCounter(
            options: new WordCloudOptions
            {
                IgnoreCase = false
            });

        const string input = "Hello hello";

        var expected = new[]
        {
            new WordFrequency("Hello", 1),
            new WordFrequency("hello", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().BeEquivalentTo(
            expected,
            options => options.WithStrictOrdering());
    }

    [Fact]
    public void CountWords_ShouldIgnorePunctuation()
    {
        // Arrange
        var counter = new WordCloudCounter();
        const string input = "hello, world! hello.";

        var expected = new[]
        {
            new WordFrequency("hello", 2),
            new WordFrequency("world", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void CountWords_ShouldSortByFrequencyDescendingThenAlphabetically()
    {
        // Arrange
        var counter = new WordCloudCounter();
        const string input = "banana apple banana cherry apple banana";

        var expected = new[]
        {
            new WordFrequency("banana", 3),
            new WordFrequency("apple", 2),
            new WordFrequency("cherry", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void CountWords_ShouldSortAlphabetically_WhenConfigured()
    {
        // Arrange
        var counter = new WordCloudCounter(
            options: new WordCloudOptions
            {
                SortMode = WordSortMode.Alphabetical
            });

        const string input = "banana apple cherry";

        var expected = new[]
        {
            new WordFrequency("apple", 1),
            new WordFrequency("banana", 1),
            new WordFrequency("cherry", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void CountWords_ShouldExcludeStopWords()
    {
        // Arrange
        var counter = new WordCloudCounter(
            options: new WordCloudOptions
            {
                StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "the",
                    "and"
                }
            });

        const string input = "the quick brown fox and the dog";

        var expected = new[]
        {
            new WordFrequency("brown", 1),
            new WordFrequency("dog", 1),
            new WordFrequency("fox", 1),
            new WordFrequency("quick", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void CountWords_ShouldRespectMinimumWordLength()
    {
        // Arrange
        var counter = new WordCloudCounter(
            options: new WordCloudOptions
            {
                MinWordLength = 3
            });

        const string input = "a an ant bear";

        var expected = new[]
        {
            new WordFrequency("ant", 1),
            new WordFrequency("bear", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void CountWords_ShouldSupportUnicodeWords()
    {
        // Arrange
        var counter = new WordCloudCounter();
        const string input = "café café naïve";

        var expected = new[]
        {
            new WordFrequency("café", 2),
            new WordFrequency("naïve", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void CountWords_ShouldSupportHyphenatedWords()
    {
        // Arrange
        var counter = new WordCloudCounter();
        const string input = "state-of-the-art state-of-the-art solution";

        var expected = new[]
        {
            new WordFrequency("state-of-the-art", 2),
            new WordFrequency("solution", 1)
        };

        // Act
        var result = counter.CountWords(input);

        // Assert
        result.Should().Equal(expected);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenMinimumWordLengthIsInvalid()
    {
        // Arrange
        var options = new WordCloudOptions
        {
            MinWordLength = 0
        };

        // Act
        var act = () => new WordCloudCounter(options: options);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>();
    }
}