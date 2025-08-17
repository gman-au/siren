namespace Siren.Infrastructure.Rendering
{
    public static class MermaidConstants
    {
        public const string SirenAnchorStart = "<!--- SIREN_START -->";
        public const string SirenAnchorEnd = "<!--- SIREN_END -->"; 
        public const string MermaidBlockStyleDefault = "default";
        public const string MermaidBlockStyleColons = "colons";
        public const string MermaidDefaultBlockBegin = "```mermaid";
        public const string MermaidDefaultBlockEnd = "```";
        public const string MermaidColonBlockBegin = ":::mermaid";
        public const string MermaidColonBlockEnd = ":::";
        public const string MermaidErDiagramHeader = "erDiagram";
        public const string MermaidNeutralThemeLine = "%%{init: {'theme':'neutral'}}%%";
    }
}
