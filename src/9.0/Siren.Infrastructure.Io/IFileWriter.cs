using System.Text;

namespace Siren.Infrastructure.Io
{
    public interface IFileWriter
    {
        void Perform(string filePath, StringBuilder result, string markdownAnchor = null);
    }
}
