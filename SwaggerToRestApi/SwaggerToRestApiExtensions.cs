// ReSharper disable UnusedMember.Global
using System;
using System.Linq;
using Fluid;
using System.Reflection;
using Microsoft.OpenApi.Models;
using OpenApiExtended;
using SwaggerToRestApi.Templates;
using SharpYaml;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace SwaggerToRestApi
{
    public static class SwaggerToRestApiExtensions
    {
        private static readonly FluidParser Parser = new FluidParser();
        public static string ToRestApi(this OpenApiDocument openApiDocument, Setting setting = null)
        {
            if (openApiDocument == null) throw new ArgumentNullException(nameof(openApiDocument));
            setting = setting ?? new Setting();
            var schemes = openApiDocument.GetOpenApiSecuritySchemes(x => x.Scheme == "bearer");
            var status = schemes.Count > 0;
            /* if (status && axiosSetting.BearerToken == null)
                 throw new ArgumentNullException("BearerToken", "OpenApi document requires a bearer token setting.");
            */

            var sourceTemplate = "";
            switch (setting.TemplateType)
            {
                case TemplateType.Axios:
                    sourceTemplate = ClientTemplate.AxiosTemplate;
                    break;
                case TemplateType.Fetch:
                    sourceTemplate = ClientTemplate.FetchTemplate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var options = new TemplateOptions();
            options.MemberAccessStrategy.Register<Setting>();

            var parsingStatus = Parser.TryParse(sourceTemplate, out var template, out var error);
            var context = new TemplateContext(options);
            context.SetValue("stg", setting);
            context.SetValue("custom_comments", CommentProvider(setting.CustomComment));
            var result = template.Render(context).Trim();

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


                    }

                }
            }

            return null;
        }

        private static string[] CommentProvider(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return null;
            }
            var lines = comment.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            return lines;
        }
    }
}
