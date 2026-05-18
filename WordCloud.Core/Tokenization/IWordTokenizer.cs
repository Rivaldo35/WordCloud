namespace WordCloud.Core.Tokenization;

public interface IWordTokenizer
{
    IEnumerable<string> Tokenize(string text);
}