using Autofac.Core;
using EvE.Endpoints;
using EveMarket.Configuration;
using EveMarket.HttpClients;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using System.Web;


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

    builder.Services.Configure<EveOptions>(builder.Configuration.GetSection(nameof(EveOptions)));
    builder.Services.AddSingleton<IOptionsMonitor<EveOptions>, OptionsMonitor<EveOptions>>();

    builder.Services.AddServices(builder.Configuration);

    //builder.Services.AddAuthorization();
    //builder.Services.AddAuthentication("Bearer").AddJwtBearer();

    var app = builder.Build();
    var eveOptions = builder.Services.BuildServiceProvider().GetRequiredService<IOptionsMonitor<EveOptions>>().CurrentValue;

    // Add the custom redirection middleware
    /*
    app.Use(async (context, next) =>
    {
        if (context.Request.Path == "/oauth-callback")
        {
            var code = context.Request.Query["code"].ToString();
            eveOptions.Code = code;

            context.Response.Redirect("/swagger/index.html");
            return;
        }
        await next();
    });
    */
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });

    var option = new RewriteOptions();
    //option.AddRedirect("^$", "swagger");
    var encodedUrl = HttpUtility.UrlEncode(eveOptions!.CallbackUrl);
    option.AddRedirect("^$", $"{eveOptions.AuthUrl}?response_type=code&redirect_uri={encodedUrl}&client_id={eveOptions.ClientId}&scope=esi-industry.read_character_jobs.v1&code_challenge={eveOptions.SecretKey}&code_challenge_method=S256&state={eveOptions.State}");
    app.UseRewriter(option);

    app.UseHttpsRedirection();

    app.MapEndpoints();

    app.Run();

}
catch (Exception)
{
    
	throw;
}

