using WordCloud.Core;
using WordCloud.Core.Options;

var options = new WordCloudOptions
{
    IgnoreCase = true,
    MinWordLength = 1,
    StopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "de",
        "het",
        "een",
        "en",
        "of",
        "the",
        "a",
        "an"
    }
};

var counter = new WordCloudCounter(options: options);

Console.WriteLine("WordCloud Counter");
Console.WriteLine("-----------------");
Console.WriteLine("Enter text below.");
Console.WriteLine("Press ENTER on an empty line to process the text.");
Console.WriteLine("Type EXIT to close the application.");
Console.WriteLine();

while (true)
{
    var lines = new List<string>();

    while (true)
    {
        var line = Console.ReadLine();

        if (string.Equals(line, "EXIT", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Closing application...");
            return;
        }

        if (string.IsNullOrWhiteSpace(line))
        {
            break;
        }

        lines.Add(line);
    }

    if (lines.Count == 0)
    {
        Console.WriteLine("No input provided.");
        Console.WriteLine();
        continue;
    }

    var text = string.Join(Environment.NewLine, lines);

    try
    {
        var result = counter.CountWords(text);

        Console.WriteLine();
        Console.WriteLine("Word frequencies:");
        Console.WriteLine("-----------------");

        foreach (var item in result)
        {
            Console.WriteLine($"{item.Word}: {item.Count}");
        }

        Console.WriteLine();
        Console.WriteLine("Enter more text or type EXIT to quit.");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected error: {ex.Message}");
        Console.WriteLine();
    }
}