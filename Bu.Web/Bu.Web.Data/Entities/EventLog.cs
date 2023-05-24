namespace Bu.Web.Data.Entities;

[Table(nameof(WebDbContext.EventLogs), Schema = "logs")]
public sealed class EventLog : BaseEntity
{
    public enum SourceApplications
    {
        [Description("Admin")]
        Admin = 1,
        [Description("Website")]
        Website = 2,
    }

    [DisplayName("Id")]
    public int EventLogId { get; set; }

    [DisplayName("Timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [DisplayName("Application")]
    [EnumDataType(typeof(SourceApplications))]
    public SourceApplications SourceApplication { get; set; }

    [DisplayName("Level")]
    [EnumDataType(typeof(LogEventLevel))]
    public LogEventLevel Level { get; set; }

    [DisplayName("Message Template")]
    [Unicode]
    public string? MessageTemplate { get; set; }

    [DisplayName("Message")]
    [Unicode]
    public string? Message { get; set; }

    [DisplayName("Exception")]
    [Unicode]
    public string? Exception { get; set; }

    [DisplayName("Log Event")]
    [Unicode]
    public string? LogEvent { get; set; }

    [DisplayName("Source Context")]
    [Unicode]
    public string? SourceContext { get; set; }

    public override int Id => this.EventLogId;

    public static void OnModelCreating(EntityTypeBuilder<EventLog> entity, string tableName)
    {
        PrimaryKey(entity, tableName, nameof(EventLogId));
        _ = entity.Property(e => e.SourceApplication).HasConversion<byte>();
        _ = entity.Property(e => e.Level).HasConversion<byte>();
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        const int width = 14;
        _ = sb.AppendLine($"{"Id",width}: {this.EventLogId}");
        _ = sb.AppendLine($"{"Event Date",width}: {this.Timestamp.ToString("O")}");
        _ = sb.AppendLine($"{"Application",width}: {this.SourceApplication}");
        _ = sb.AppendLine($"{"Level",width}: {this.Level}");
        _ = sb.AppendLine($"{"Source Context",width}: {this.SourceContext}");
        _ = sb.AppendLine();
        _ = sb.AppendLine("▶ Message");
        _ = sb.AppendLine(this.Message);
        _ = sb.AppendLine();
        _ = sb.AppendLine("▶ Message Template");
        _ = sb.AppendLine(this.MessageTemplate);
        _ = sb.AppendLine();
        _ = sb.AppendLine("▶ Exception");
        _ = sb.AppendLine(this.Exception);
        _ = sb.AppendLine();
        _ = sb.AppendLine("▶ Log Event");
        _ = sb.AppendLine(this.LogEvent);
        return sb.ToString();
    }

    public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        yield break;
    }
}