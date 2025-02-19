namespace TrelloApi.Domain.Constants;

internal static class UserThemeValues
{
    private const string Dark = "Dark";
    private const string Light = "Light";

    public static readonly List<string> UserThemesAllowed = new List<string> { Dark, Light };
}