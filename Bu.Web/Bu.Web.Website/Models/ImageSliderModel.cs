namespace Bu.Web.Website.Models;

public class ImageSliderModel
{
    public IEnumerable<ImageModel> SliderImages { get; set; } = new List<ImageModel>();
}