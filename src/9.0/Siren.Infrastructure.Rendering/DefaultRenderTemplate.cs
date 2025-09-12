using Siren.Interfaces;

namespace Siren.Infrastructure.Rendering
{
    public class DefaultRenderTemplate(IProgramArguments programArguments) : IRenderTemplate
    {
        public bool IsApplicable() => programArguments?.RenderTemplate?.ToLower() == "default";
        public string ThemeLine => "%%{init: {'theme':'neutral'}}%%";
        public string MermaidBlockStart => "```mermaid";
        public string MermaidBlockEnd => "```";
    }
}