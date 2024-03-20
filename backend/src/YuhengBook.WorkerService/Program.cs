using Serilog;
using Serilog.Extensions.Logging;
using YuhengBook.Infrastructure;
using YuhengBook.WorkerService;
using YuhengBook.WorkerService.Services;

Environment.CurrentDirectory = AppContext.BaseDirectory;

var builder = Host.CreateApplicationBuilder(args);

var logger = Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(builder.Configuration)
   .Enrich.FromLogContext()
   .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var appLogger = new SerilogLoggerFactory(logger)
   .CreateLogger<Program>();

builder.Services.Configure<ChapterFormatOption>(
    builder.Configuration.GetSection(nameof(ChapterFormatOption))
);

builder.Services.AddHostedService<BqgNovelsWorker>();
// builder.Services.AddHostedService<ChapterFormatWorker>();

builder.Services.AddHttpClient();

builder.Services.AddInfrastructureServices(
    builder.Configuration,
    appLogger,
    builder.Environment.EnvironmentName
);

var host = builder.Build();

appLogger.LogInformation("Starting service host");
host.Run();
appLogger.LogInformation("Service host stopped");
