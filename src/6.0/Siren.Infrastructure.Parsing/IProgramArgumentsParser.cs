using Siren.Domain;

namespace Siren.Infrastructure.Parsing
{
    public interface IProgramArgumentsParser
    {
        ProgramArguments Parse(string[] args);
    }
}