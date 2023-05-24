namespace Bu.Web.Website.Models;

public class MainWelcomeSectionModel
{
    public IEnumerable<CampusModel> Campuses { get; set; } = new List<CampusModel>();

    public WelcomeMessageModel WelcomeMessage { get; set; } = new WelcomeMessageModel
    {
        Title = string.Empty,
        Message = string.Empty,
    };
}