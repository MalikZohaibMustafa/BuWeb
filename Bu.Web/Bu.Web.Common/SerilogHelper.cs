using System.Data;
using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

namespace Bu.Web.Common;

public static class SerilogHelper
{
    public static LoggerConfiguration ConfigureSerilog(ILogger logger, WebApplicationBuilder builder, LoggerConfiguration loggerConfiguration, string connectionString, EventLog.SourceApplications sourceApplication)
    {
        static ColumnOptions CreateColumnOptions()
        {
            var columnOptions = new ColumnOptions
            {
                Id =
                {
                    ColumnName = nameof(EventLog.EventLogId),
                    DataType = SqlDbType.Int,
                    AllowNull = false,
                },
                TimeStamp =
                {
                    ColumnName = nameof(EventLog.Timestamp),
                    DataType = SqlDbType.DateTimeOffset,
                    AllowNull = false,
                    ConvertToUtc = false,
                },
                Level =
                {
                    ColumnName = nameof(EventLog.Level),
                    AllowNull = false,
                    DataType = SqlDbType.TinyInt,
                    StoreAsEnum = true,
                },
                MessageTemplate =
                {
                    ColumnName = nameof(EventLog.MessageTemplate),
                    DataType = SqlDbType.NVarChar,
                    AllowNull = true,
                    DataLength = -1,
                },
                Message =
                {
                    ColumnName = nameof(EventLog.Message),
                    DataType = SqlDbType.NVarChar,
                    AllowNull = true,
                    DataLength = -1,
                },
                Exception =
                {
                    ColumnName = nameof(EventLog.Exception),
                    DataType = SqlDbType.NVarChar,
                    AllowNull = true,
                    DataLength = -1,
                },
                LogEvent =
                {
                    ColumnName = nameof(EventLog.LogEvent),
                    DataType = SqlDbType.NVarChar,
                    AllowNull = true,
                    DataLength = -1,
                },
                AdditionalColumns = new List<SqlColumn>
                {
                    new SqlColumn(nameof(EventLog.SourceContext), SqlDbType.NVarChar, dataLength: -1),
                    new SqlColumn(nameof(EventLog.SourceApplication), SqlDbType.TinyInt, allowNull: false),
                },
            };

            columnOptions.Store.Clear();
            columnOptions.Store.Add(StandardColumn.Id);
            columnOptions.Store.Add(StandardColumn.TimeStamp);
            columnOptions.Store.Add(StandardColumn.Level);
            columnOptions.Store.Add(StandardColumn.MessageTemplate);
            columnOptions.Store.Add(StandardColumn.Message);
            columnOptions.Store.Add(StandardColumn.Exception);
            columnOptions.Store.Add(StandardColumn.LogEvent);
            return columnOptions;
        }

        LogEventLevel minimumLoggingLevel = LogEventLevel.Verbose;
        if (builder.Environment.IsProduction())
        {
            minimumLoggingLevel = LogEventLevel.Warning;
        }

        minimumLoggingLevel = builder.Configuration.GetValue("MinimumLoggingLevel", minimumLoggingLevel);

        logger.Information("Minimum Logging Level {LoggingLevel} is configured.", minimumLoggingLevel);
        return loggerConfiguration.WriteTo.Console()
            .MinimumLevel.ControlledBy(new LoggingLevelSwitch
            {
                MinimumLevel = minimumLoggingLevel,
            })
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .Enrich.WithProperty(nameof(EventLog.SourceApplication), (byte)sourceApplication)
            .WriteTo.MSSqlServer(
                connectionString,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    AutoCreateSqlTable = false,
                    SchemaName = "logs",
                    TableName = nameof(ICommonContext.EventLogs),
                },
                columnOptions: CreateColumnOptions());
    }
}