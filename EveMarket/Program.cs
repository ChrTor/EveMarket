using Autofac.Core;
using EvE.Endpoints;
using EveMarket.Configuration;
using EveMarket.HttpClients;
using Microsoft.AspNetCore.Rewrite;
using System.Text.Json.Serialization;


try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllersWithViews()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.CustomSchemaIds(type => type.DeclaringType != null ? $"{type.DeclaringType.Name}.{type.Name}" : type.Name);
        c.SchemaFilter<SortEnumValuesByNameSchemaFilter>();
    });
    builder.Services.AddServices(builder.Configuration);
    builder.Services.AddHttpClient();
    builder.Services.AddTransient<EveClient>();

    //builder.Services.AddAuthorization();
    //builder.Services.AddAuthentication("Bearer").AddJwtBearer();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    var option = new RewriteOptions();
    option.AddRedirect("^$", "swagger");
    app.UseRewriter(option);

    app.UseHttpsRedirection();

    app.MapEndpoints();

    app.Run();

}
catch (Exception)
{
    
	throw;
}

