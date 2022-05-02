
// ReSharper disable CheckNamespace

namespace SwaggerToRestApi
{
    public class Setting
    {
        public string ClientClassName { get; set; } = "Client";
        public string CustomComment { get; set; } = "";
        public BearerToken BearerToken { get; set; }
        public TemplateType TemplateType { get; set; } = TemplateType.Fetch;
    }
}
