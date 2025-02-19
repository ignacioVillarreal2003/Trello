namespace TrelloApi.Domain.Constants;

internal static class RoleValues
{
    private const string Admin = "Admin";
    private const string Member = "Member";

    public static readonly List<string> RolesAllowed = new List<string> { Admin, Member };
}