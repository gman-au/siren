using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Siren.Domain;
using Siren.Infrastructure.AssemblyLoad.Builders;

namespace Siren.Infrastructure.AssemblyLoad
{
    public class AssemblyLoader : IAssemblyLoader
    {
        private readonly ILogger<AssemblyLoader> _logger;
        private readonly IEntityBuilder _entityBuilder;

        public AssemblyLoader(
            ILogger<AssemblyLoader> logger, 
            IEntityBuilder entityBuilder)
        {
            _logger = logger;
            _entityBuilder = entityBuilder;
        }

        public Universe Perform(ProgramArguments arguments)
        {
            var filePath =
                arguments
                    .TestAssemblyPath;

            var assembly =
                AssemblyDefinition
                    .ReadAssembly(filePath);

            foreach (var module in assembly.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.BaseType?.Name == "ModelSnapshot")
                    {
                        _logger
                            .LogInformation($"Located snapshot type {type.Name}");

                        // delete me
                        var instrData = new StringBuilder();

                        foreach (var method in type.Methods)
                        {
                            if (method.Name != "BuildModel") continue;
                            var counter = 0;
                            // delete me
                            var indent = 0;

                            var entityInstructions =
                                method
                                    .Body
                                    .Instructions
                                    .Where(o => _entityBuilder.IsApplicable(o))
                                    .ToList();

                            var entities =
                                entityInstructions
                                    .Select(o => _entityBuilder.Process(o))
                                    .ToList();
                            
                            foreach (var instr in method.Body.Instructions)
                            {
                                ProcessCallVirt(
                                    instr,
                                    instrData,
                                    ref indent
                                );

                                ProcessLdftn(
                                    instr,
                                    instrData,
                                    ref indent
                                );

                                instrData
                                    .Append(
                                        instr.OpCode.ToString().PadRight(
                                            10,
                                            ' '
                                        )
                                    )
                                    .Append("\t: ")
                                    .Append(instr?.Operand)
                                    .AppendLine();
                            }

                            File.WriteAllText(
                                $"C:\\SF2MD_DATA\\{type.Name}.txt",
                                instrData.ToString()
                            );
                        }
                    }
                }
            }

            return null;
        }

        private void ProcessCallVirt(Instruction instr, StringBuilder builder, ref int indent)
        {
            if (instr.OpCode == OpCodes.Callvirt)
            {
                if (instr.Operand is not MethodDefinition methodReference) return;

                indent++;
                foreach (var methInstr in methodReference.Body.Instructions)
                {
                    for (int i = 0; i < indent; i++)
                    {
                        builder
                            .Append("\t");
                    }

                    builder
                        .Append(
                            methInstr.OpCode.ToString().PadRight(
                                10,
                                ' '
                            )
                        )
                        .Append("\t: ")
                        .Append(methInstr?.Operand)
                        .AppendLine();

                    ProcessCallVirt(
                        methInstr,
                        builder,
                        ref indent
                    );
                    ProcessLdftn(
                        methInstr,
                        builder,
                        ref indent
                    );
                }

                indent--;
            }
        }

        private void ProcessLdftn(Instruction instr, StringBuilder builder, ref int indent)
        {
            if (instr.OpCode == OpCodes.Ldftn)
            {
                if (instr.Operand is not MethodDefinition methodReference) return;

                indent++;
                foreach (var methInstr in methodReference.Body.Instructions)
                {
                    for (int i = 0; i < indent; i++)
                    {
                        builder
                            .Append("\t");
                    }

                    builder
                        .Append(
                            methInstr.OpCode.ToString().PadRight(
                                10,
                                ' '
                            )
                        )
                        .Append("\t: ")
                        .Append(methInstr?.Operand)
                        .AppendLine();


                    ProcessCallVirt(
                        methInstr,
                        builder,
                        ref indent
                    );
                    ProcessLdftn(
                        methInstr,
                        builder,
                        ref indent
                    );
                }

                indent--;
            }
        }
    }
}