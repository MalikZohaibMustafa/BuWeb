namespace Bu.Web.Website.Models;

public class AnnouncementModel
{
    public IEnumerable<Announcement> Announcements { get; set; } = new List<Announcement>();
}