using Siren.Interfaces;

namespace Siren.Infrastructure.Rendering;

public class AzureDevOpsRenderTemplate(IProgramArguments programArguments) : IRenderTemplate
{
    public bool IsApplicable() => programArguments.RenderTemplate.ToLower() == "azuredevops";
    public string ThemeLine => null; 
    public string MermaidBlockStart => ":::mermaid";
    public string MermaidBlockEnd => ":::";
}