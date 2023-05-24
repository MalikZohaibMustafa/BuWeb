using Serilog.Events;

namespace Bu.Web.Admin.Areas.Admin.Models.EventLogs;

public sealed class ListModel : SortedPageDataModel<EventLog, ListInputModel>
{
    public ListModel(ListInputModel input, ClaimsPrincipal user)
        : base(input, user)
    {
    }

    public ListModel(AdminContextProvider adminContextProvider, ListInputModel input, ClaimsPrincipal user)
        : base(adminContextProvider, input, user)
    {
    }

    public string GetCssClass(LogEventLevel logLevel)
    {
        return logLevel switch
        {
            LogEventLevel.Verbose => string.Empty,
            LogEventLevel.Debug => string.Empty,
            LogEventLevel.Information => string.Empty,
            LogEventLevel.Warning => "table-warning",
            LogEventLevel.Error => "table-danger",
            LogEventLevel.Fatal => "table-danger",
            _ => string.Empty,
        };
    }

    protected override IQueryable<EventLog> GetFilteredQuery(IAdminContext adminContext)
    {
        var query = adminContext.EventLogs.AsQueryable();

        if (this.Input.SourceApplication != null)
        {
            query = query.Where(e => e.SourceApplication == this.Input.SourceApplication.Value);
        }

        if (this.Input.Level != null)
        {
            query = query.Where(e => e.Level == this.Input.Level.Value);
        }

        if (this.Input.From != null)
        {
            var from = this.Input.From.Value.AttachUserDateTimeOffset(this.User);
            query = query.Where(e => e.Timestamp >= from);
        }

        if (this.Input.To != null)
        {
            var to = this.Input.To.Value.AttachUserDateTimeOffset(this.User);
            query = query.Where(e => e.Timestamp <= to);
        }

        return query;
    }

    protected override IOrderedQueryable<EventLog> PrepareSortedQuery(IQueryable<EventLog> query)
    {
        return this.Input.Sort switch
        {
            nameof(EventLog.EventLogId) => query.OrderBy(this.Input.Desc, e => e.EventLogId),
            nameof(EventLog.Timestamp) => query.OrderBy(this.Input.Desc, e => e.Timestamp),
            _ => throw new NotSupportedException(this.Input.Sort),
        };
    }
}