namespace TrelloApi.Domain.Constants;

public class UserThemeValues
{
    private const string Dark = "Dark";
    private const string Light = "Light";

    public static readonly List<string> Allowed = new List<string> { Dark, Light };
}