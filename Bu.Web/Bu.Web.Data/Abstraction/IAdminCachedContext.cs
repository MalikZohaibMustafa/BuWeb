namespace Bu.Web.Data.Abstraction;

public interface IAdminCachedContext : ICommonCachedContext
{
    public ILookup<string, User> UsersByEmail { get; }

    public ILookup<int, User> UsersByUserId { get; }

    public IReadOnlyList<UserArea> UserAreas { get; }

    public ILookup<int, UserRole> UserRoles { get; }

    public string? GetUserInfo(int? userId, DateTime? userDataTime)
    {
        if (userId == null)
        {
            return userDataTime?.ToString();
        }

        string createdBy = this.UsersByUserId[userId.Value].SingleOrDefault()?.ToString() ?? "N/A";
        return userDataTime == null ? createdBy : $"{createdBy} @ {userDataTime.Value}";
    }
}