using DibBase.Infrastructure;
using DsCore.ApiClient;
using DsCore.Api.Options;
using DsCore.Infrastructure;
using DsStorage.ApiClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDocument();

builder.Services.AddDbContext<DbContext, DsCoreContext>();
var entityTypes = new List<Type>();
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
DsCore.Api.Models.User a;//█▬█ █ ▀█▀
foreach (var assembly in assemblies)
{
    entityTypes.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(DibBase.ModelBase.Entity))).ToList());
    foreach (var e in entityTypes)
    {
        var repositoryType = typeof(Repository<>).MakeGenericType(e);
        builder.Services.AddScoped(repositoryType);
    }
}
builder.Services.AddOptions<TokenOptions>()
    .Bind(builder.Configuration.GetSection(TokenOptions.SECTION))
    .ValidateDataAnnotations();
builder.Configuration.AddDsStorage(builder.Services);
builder.Services.AddAuthorization();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(builder =>
    builder.WithOrigins("http://localhost:1420", "http://localhost:5215", "http://localhost:5216", "http://localhost:5217")
            .AllowAnyHeader()
            .AllowAnyMethod()
);
app.MapControllers();
app.Run();