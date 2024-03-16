//using Elastic.Serilog.Sinks;
using Serilog;
using Serilog.Debugging;
using Serilog.Sinks.Elasticsearch;
using SmartApp.Handlers;
using SmartApp.Models;
using SmartApp.RabbitMQ;
using SmartApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthorsService, AuthorsService>();
builder.Services.AddScoped<IBlackListService, BlackListService>();
builder.Services.AddScoped<IParserService, ParserService>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();
builder.Services.AddHostedService<RabbitMqListener>(); 
 
builder.Services.AddHostedService<LoadingService>();

builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
    loggerConfiguration
        .WriteTo.Console()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://elastic:MyPw123@localhost:9200"))
        {
            TypeName = null,
            AutoRegisterTemplate = true,
            IndexFormat = "WriteToElasticsearchWithSerilog-{0:yyyy-MM-dd}",
        }));
SelfLog.Enable(Console.Error);

builder.Services.Configure<SmartDBConnection>(builder.Configuration.GetSection("ConnectionDBSmart"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMqConnection"));

var messageHandlerRegistrator = new MessageHandlerRegistrator(builder.Services.BuildServiceProvider());
messageHandlerRegistrator.Register("DataUpdateRequest", typeof(LoadingCommentsHandler));
messageHandlerRegistrator.Register("DataUpdateRequest", typeof(LogginLoadCommentsDBHandler));
// messageHandlerRegistrator.Register("DataUpdateRequest", typeof(LogginLoadCommentsDBHandler));
builder.Services.AddSingleton(messageHandlerRegistrator);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

