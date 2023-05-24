namespace Bu.Web.Admin.Models;

internal static class Extensions
{
    public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> entities, INamedEntity.Names name, bool appendStaticListItem, string staticListItemText = "", string staticListItemValue = "")
        where T : INamedEntity
    {
        IEnumerable<SelectListItem> selectListItems = entities.Select(e => e.ToSelectListItem(name));
        return appendStaticListItem
            ? new[] { new SelectListItem(staticListItemText, staticListItemValue) }.Concat(selectListItems)
            : selectListItems;
    }
}