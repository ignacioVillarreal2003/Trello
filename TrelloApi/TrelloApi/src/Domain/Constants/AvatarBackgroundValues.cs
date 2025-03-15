namespace TrelloApi.Domain.Constants;

internal static class AvatarBackgroundValues
{
    private const string Red = "var(--color-red-500)";
    private const string Yellow = "var(--color-yellow-500)";
    private const string Green = "var(--color-green-500)";
    private const string Skyblue = "var(--color-skyblue-500)";
    private const string Blue = "var(--color-blue-500)";
    private const string Orange = "var(--color-orange-500)";
    
    public static readonly List<string> AvatarBackgroundsAllowed = new List<string> { Red, Yellow, Green, Skyblue, Blue, Orange };
}