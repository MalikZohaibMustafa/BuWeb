using Serilog.Events;
using static Bu.Web.Data.Entities.EventLog;

namespace Bu.Web.Admin.Areas.Admin.Models.EventLogs;

public sealed class ListInputModel : BaseInputModel, ISort, IPage
{
    public string? Sort { get; set; } = nameof(EventLog.EventLogId);

    public bool? Desc { get; set; } = true;

    public string? SortQueryParameterName => "Input.Sort";

    public string? DescQueryParameterName => "Input.Desc";

    public int? PageIndex { get; set; }

    public int? PageSize { get; set; }

    public string? PageIndexQueryParameterName => "Input.PageIndex";

    public string? PageSizeQueryParameterName => "Input.PageSize";

    [DisplayName("Application")]
    public SourceApplications? SourceApplication { get; init; }

    [DisplayName("Level")]
    public LogEventLevel? Level { get; init; }

    [DisplayName("From")]
    public DateTime? From { get; init; }

    [DisplayName("To")]
    [CompareOtherPropertyValue(nameof(From), CompareOtherPropertyValueAttribute.DataTypes.DateTime, CompareOtherPropertyValueAttribute.ComparisonTypes.GreaterThanOrEqualTo)]
    public DateTime? To { get; init; }
}