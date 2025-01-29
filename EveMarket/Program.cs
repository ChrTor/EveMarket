using ErrorOr;
using EvE.Endpoints;
using EveMarket.Configuration;
using Microsoft.AspNetCore.Rewrite;
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
        c.SchemaFilter<SortEnumValuesByNameSchemaFilter>();
    });

    builder.Services.AddOptions<EveOptions>()
        .Bind(builder.Configuration.GetSection("EveOptions"));
    builder.Services.AddSingleton<IOptionsMonitor<EveOptions>, OptionsMonitor<EveOptions>>();

    builder.Services.AddServices(builder.Configuration);



    //builder.Services.AddAuthorization();
    //builder.Services.AddAuthentication("Bearer").AddJwtBearer();

    var app = builder.Build();

    app
        .UseRouting()
        .UseSwagger()
        .UseSwaggerUI(o =>
        {
            o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        });
    app.MapQueryEndpoints();
    app.MapCallBackEndpoint();

    var eveOptions = app.Services.GetRequiredService<IOptionsMonitor<EveOptions>>().CurrentValue;


    var option = new RewriteOptions();
    // Testing Player Auth
    var encodedUrl = HttpUtility.UrlEncode(eveOptions!.CallbackUrl);
    //option.AddRedirect("^$", "swagger");
    option.AddRedirect("^$", $"{eveOptions.AuthUrl}?response_type=code&redirect_uri={encodedUrl}&client_id={eveOptions.ClientId}&scope={eveOptions.EnabledScopes[0].Address}&code_challenge={eveOptions.SecretKey}&code_challenge_method=S256&state={eveOptions.State}");
    
    app.UseRewriter(option);
    app.UseHttpsRedirection();
    app.Run();

}
catch (Exception)
{
    
	throw;
}

