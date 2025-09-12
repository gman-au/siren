namespace Siren.Infrastructure.Rendering
{
    public interface IRenderTemplate
    {
        bool IsApplicable();
        string ThemeLine { get; }
        string MermaidBlockStart { get; }
        string MermaidBlockEnd { get; }
    }
}