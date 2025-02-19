namespace TrelloApi.Domain.Constants;

internal static class BoardBackgroundValues
{
    private const string Background1 = "background-1.svg";
    private const string Background2 = "background-2.svg";
    private const string Background3 = "background-3.svg";
    private const string Background4 = "background-4.svg";
    private const string Background5 = "background-5.svg";
    private const string Background6 = "background-6.svg";
    
    public static readonly List<string> BoardBackgroundsAllowed = new List<string> { Background1, Background2, Background3, Background4, Background5, Background6 };
}