using System.Linq;
using System.Web.Http;
using AiDollar.ApiGateway.Http;
using Swashbuckle.Application;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace AiDollar.ApiGateway.Http
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration configuration)
        {
            configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "AiDollar.ApiGateway");
                        
                        c.ApiKey("Bearer")
                            .Description("Filling bearer token here")
                            .Name("Authorization")
                            .In("header");

                        c.ResolveConflictingActions(apiDescriptions => 
                            apiDescriptions.FirstOrDefault(r => r.ParameterDescriptions.Count > 0));
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DisableValidator();
                        c.EnableApiKeySupport("Authorization", "header");
                    });
        }
    }
}
