namespace TrelloApi.Domain;

public class BoardThemeValues
{
    private const string Blue = "Blue";
    private const string Light = "Light";

    public static readonly List<string> Allowed = new List<string> { Blue, Light };
}