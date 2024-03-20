using System.Text;

namespace YuhengBook.WorkerService;

public class ChapterFormatOption
{
    public string[] SplitPatterns { get; set; } = [];

    public string[] AdsArray { get; set; } = [];

    public string SplitSymbol { get; set; } = "\n\n";
}

public static class ChapterContentExtensions
{
    private static readonly string[] DefaultAdsArray = ["\0"];

    public static string FormatContent(string text, ChapterFormatOption option)
    {
        var splitSymbol = option.SplitSymbol;


        var sb = new StringBuilder();
        sb.Append(text);

        var splitPatterns = option.SplitPatterns;
        foreach (var pattern in splitPatterns)
        {
            sb.Replace(pattern, splitSymbol);
        }

        var adsArray = option.AdsArray.Union(DefaultAdsArray);
        foreach (var ads in adsArray)
        {
            sb.Replace(ads, string.Empty);
        }

        var splits = sb.ToString()
           .Split(splitSymbol, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        return string.Join(splitSymbol, splits);
    }
}
