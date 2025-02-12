namespace TrelloApi.Domain;

public class PriorityValues
{
    private const string Low = "Low";
    private const string Medium = "Medium";
    private const string High = "High";

    public static readonly List<string> Allowed = new List<string> { Low, Medium, High };
}