namespace TrelloApi.Domain.Constants;

internal static class PriorityValues
{
    private const string Low = "Low";
    private const string Medium = "Medium";
    private const string High = "High";

    public static readonly List<string> PrioritiesAllowed = new List<string> { Low, Medium, High };
}