namespace TrelloApi.Domain.Constants;

public class RoleValues
{
    private const string Admin = "Admin";
    private const string Member = "Member";

    public static readonly List<string> Allowed = new List<string> { Admin, Member };
}