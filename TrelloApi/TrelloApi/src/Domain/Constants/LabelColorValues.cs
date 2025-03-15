namespace TrelloApi.Domain.Constants;

internal static class LabelColorValues
{
    private const string Green1 = "#75e0b0";
    private const string Green2 = "#4bce97";
    private const string Green3 = "#19b076";
    private const string Yellow1 = "#f9e48e";
    private const string Yellow2 = "#F5CD47";
    private const string Yellow3 = "#f3bd2c";
    private const string Orange1 = "#ffcfa9";
    private const string Orange2 = "#FEA362";
    private const string Orange3 = "#fd803a";
    private const string Red1 = "#fcaaa5";
    private const string Red2 = "#F87168";
    private const string Red3 = "#f04e43";
    private const string Purple1 = "#c3bcf6";
    private const string Purple2 = "#9F8FEF";
    private const string Purple3 = "#856ae8";
    private const string Blue1 = "#90c2ff";
    private const string Blue2 = "#579DFF";
    private const string Blue3 = "#357bfc";
    
    public static readonly List<string> LabelColorsAllowed = new List<string>
    {
        Green1,
        Green2,
        Green3,
        Yellow1,
        Yellow2,
        Yellow3,
        Orange1,
        Orange2,
        Orange3,
        Red1,
        Red2,
        Red3,
        Purple1,
        Purple2,
        Purple3,
        Blue1,
        Blue2,
        Blue3
    };
}