namespace TrelloApi.Domain.Constants;

internal static class BoardColorValues
{
    private const string Red = "ff6575";
    private const string Yellow = "ffd02f";
    private const string Green = "24cbc7";
    private const string Purple = "6842ef";
    private const string Blue = "0077ff";
    private const string Orange = "ff9b00";
    
    public static readonly List<string> BoardColorsAllowed = new List<string> { Red, Yellow, Green, Purple, Blue, Orange };
}