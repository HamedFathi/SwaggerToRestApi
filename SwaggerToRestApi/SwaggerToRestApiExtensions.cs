// ReSharper disable UnusedMember.Global
using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using OpenApiExtended;

namespace SwaggerToRestApi
{
    public static class SwaggerToRestApiExtensions
    {
        public static string ToRestApi(this OpenApiDocument openApiDocument, Setting setting = null)
        {
            if (openApiDocument == null) throw new ArgumentNullException(nameof(openApiDocument));
            setting = setting ?? new Setting();
            var schemes = openApiDocument.GetOpenApiSecuritySchemes(x => x.Scheme == "bearer");
            var status = schemes.Count > 0;
            /* if (status && axiosSetting.BearerToken == null)
                 throw new ArgumentNullException("BearerToken", "OpenApi document requires a bearer token setting.");
            */
            var paths = openApiDocument.Paths;
            foreach (var openApiPath in paths)
            {
                var pathKey = openApiPath.Key;
                foreach (var openApiOperation in openApiPath.Value.Operations)
                {
                    var operationKey = openApiOperation.Key;
                    var requestBodySchema = openApiOperation.Value.RequestBody?.GetResponsesContent(x =>
                        x == OpenApiMimeType.ApplicationJson || x == OpenApiMimeType.TextPlain).First().Schema;
                    var requestBodyTypeScript = requestBodySchema?.ToTypeScript(requestBodySchema.Reference.Id);

                    var responses = openApiOperation.Value.Responses;
                    foreach (var openApiResponse in responses)
                    {
                        var responseKey = openApiResponse.Key;
                        var responseContent = openApiResponse.Value.GetResponsesContent(x => x == OpenApiMimeType.ApplicationJson || x == OpenApiMimeType.TextPlain).FirstOrDefault();
                        var responseSchema = responseContent?.Schema;
                        var responseTypeScript = responseSchema?.ToTypeScript(responseSchema.Reference.Id);
                        
                        
                    }

                }
            }

            return null;
        }
    }
}
