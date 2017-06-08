using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using Sitecore.Pathfinder.Configuration;
using Sitecore.Pathfinder.Configuration.ConfigurationModel;
using Sitecore.Pathfinder.Diagnostics;
using Sitecore.Pathfinder.Extensibility;
using Sitecore.Pathfinder.Extensibility.Pipelines;
using Sitecore.Pathfinder.Extensions;
using Sitecore.Pathfinder.IO;
using Sitecore.Pathfinder.Parsing;
using Sitecore.Pathfinder.Tasks.Building;

namespace Sitecore.Pathfinder.Tasks
{
    [Export(typeof(ITask)), Shared]
    public class GenerateFactory : BuildTaskBase
    {
        [ImportingConstructor]
        public GenerateFactory() : base("generate-factory")
        {
            IsHidden = true;
        }

        public override void Run(IBuildContext context)
        {
            using (var stream = new FileStream("IFactory.generated.cs.tmp", FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("//------------------------------------------------------------------------------");
                    writer.WriteLine("// <auto-generated>");
                    writer.WriteLine("// This code was generated by a tool.");
                    writer.WriteLine("//");
                    writer.WriteLine("// Changes to this file may cause incorrect behavior and will be lost if");
                    writer.WriteLine("// the code is regenerated.");
                    writer.WriteLine("//");
                    writer.WriteLine("// </auto-generated>");
                    writer.WriteLine("//------------------------------------------------------------------------------");
                    writer.WriteLine();
                    writer.WriteLine("#pragma warning disable 1591");
                    writer.WriteLine();
                    writer.WriteLine("using Sitecore.Pathfinder.Diagnostics;");
                    writer.WriteLine();
                    writer.WriteLine("namespace Sitecore.Pathfinder.Configuration");
                    writer.WriteLine("{");
                    writer.WriteLine("    #region Designer generated code");
                    writer.WriteLine();
                    writer.WriteLine("    public partial interface IFactory");
                    writer.WriteLine("    {");

                    foreach (var type in typeof(HostService).GetTypeInfo().Assembly.GetTypes().OrderBy(t => t.Name))
                    {
                        foreach (var constructor in type.GetTypeInfo().GetConstructors())
                        {
                            var attr = constructor.GetCustomAttribute<FactoryConstructorAttribute>();
                            if (attr == null)
                            {
                                continue;
                            }

                            var returnType = type.FullName;
                            if (attr.ReturnType != null)
                            {
                                returnType = GetTypeName(attr.ReturnType);
                            }

                            var parameters = string.Empty;
                            if (constructor.GetCustomAttribute<ImportingConstructorAttribute>() != null)
                            {
                                var exportAttribute = type.GetTypeInfo().GetCustomAttributes<ExportAttribute>().FirstOrDefault();
                                if (exportAttribute != null)
                                {
                                    returnType = GetTypeName(exportAttribute.ContractType ?? type);
                                }

                                var with = type.GetTypeInfo().GetMethods().FirstOrDefault(m => m.Name == "With");
                                if (with != null)
                                {
                                    parameters = string.Join(", ", with.GetParameters().Where(p => GetSpecialType(p.ParameterType) == null).Select(p => (p.ParameterType.GetTypeInfo().IsValueType ? string.Empty : "[NotNull] ") + GetTypeName(p.ParameterType) + " " + p.Name));
                                    returnType = GetTypeName(with.ReturnType);
                                }
                            }
                            else
                            {
                                parameters = string.Join(", ", constructor.GetParameters().Where(p => GetSpecialType(p.ParameterType) == null).Select(p => (p.ParameterType.GetTypeInfo().IsValueType ? string.Empty : "[NotNull] ") + GetTypeName(p.ParameterType) + " " + p.Name).ToList());
                            }

                            writer.WriteLine("        [NotNull]");
                            writer.WriteLine($"        {returnType} {type.Name}({parameters});");
                            writer.WriteLine();
                        }
                    }

                    writer.WriteLine("    }");
                    writer.WriteLine();
                    writer.WriteLine("    #endregion");
                    writer.WriteLine("}");
                    writer.WriteLine();
                    writer.WriteLine("#pragma warning restore 1591");
                }
            }

            using (var stream = new FileStream("Factory.generated.cs.tmp", FileMode.Create))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("//------------------------------------------------------------------------------");
                    writer.WriteLine("// <auto-generated>");
                    writer.WriteLine("// This code was generated by a tool.");
                    writer.WriteLine("//");
                    writer.WriteLine("// Changes to this file may cause incorrect behavior and will be lost if");
                    writer.WriteLine("// the code is regenerated.");
                    writer.WriteLine("//");
                    writer.WriteLine("// </auto-generated>");
                    writer.WriteLine("//------------------------------------------------------------------------------");
                    writer.WriteLine();
                    writer.WriteLine("#pragma warning disable 1591");
                    writer.WriteLine();
                    writer.WriteLine("namespace Sitecore.Pathfinder.Configuration");
                    writer.WriteLine("{");
                    writer.WriteLine("    #region Designer generated code");
                    writer.WriteLine();
                    writer.WriteLine("    public partial class Factory : IFactory");
                    writer.WriteLine("    {");

                    foreach (var type in typeof(HostService).GetTypeInfo().Assembly.GetTypes().OrderBy(t => t.Name))
                    {
                        foreach (var constructor in type.GetTypeInfo().GetConstructors())
                        {
                            var attr = constructor.GetCustomAttribute<FactoryConstructorAttribute>();
                            if (attr == null)
                            {
                                continue;
                            }

                            var returnType = type.FullName;
                            if (attr.ReturnType != null)
                            {
                                returnType = GetTypeName(attr.ReturnType);
                            }

                            var parameters = string.Empty;
                            var arguments = string.Join(", ", constructor.GetParameters().Select(p => GetSpecialType(p.ParameterType) ?? p.Name));
                            var statement = $"new {type.FullName}({arguments})";

                            if (constructor.GetCustomAttribute<ImportingConstructorAttribute>() != null)
                            {
                                var exportAttribute = type.GetTypeInfo().GetCustomAttributes<ExportAttribute>().FirstOrDefault();
                                if (exportAttribute != null)
                                {
                                    returnType = GetTypeName(exportAttribute.ContractType ?? type);
                                    statement = "Resolve<" + returnType + ">()";
                                }

                                var with = type.GetTypeInfo().GetMethods().FirstOrDefault(m => m.Name == "With");
                                if (with != null)
                                {
                                    parameters = string.Join(", ", with.GetParameters().Where(p => GetSpecialType(p.ParameterType) == null).Select(p => GetTypeName(p.ParameterType) + " " + p.Name));
                                    arguments = string.Join(", ", with.GetParameters().Select(p => GetSpecialType(p.ParameterType) ?? p.Name));
                                    statement += $".With({arguments})";
                                    returnType = GetTypeName(with.ReturnType);
                                }
                            }
                            else
                            {
                                parameters = string.Join(", ", constructor.GetParameters().Where(p => GetSpecialType(p.ParameterType) == null).Select(p => GetTypeName(p.ParameterType) + " " + p.Name).ToList());
                            }

                            writer.WriteLine($"        public virtual {returnType} {type.Name}({parameters}) => {statement};");
                            writer.WriteLine();
                        }
                    }

                    writer.WriteLine("    }");
                    writer.WriteLine();
                    writer.WriteLine("    #endregion");
                    writer.WriteLine("}");
                    writer.WriteLine();
                    writer.WriteLine("#pragma warning restore 1591");
                }

                if (File.Exists("Factory.generated.cs"))
                {
                    File.Delete("Factory.generated.cs.bak");
                    File.Move("Factory.generated.cs", "Factory.generated.cs.bak");
                }

                if (File.Exists("IFactory.generated.cs"))
                {
                    File.Delete("IFactory.generated.cs.bak");
                    File.Move("IFactory.generated.cs", "IFactory.generated.cs.bak");
                }

                File.Move("IFactory.generated.cs.tmp", "IFactory.generated.cs");

                File.Move("Factory.generated.cs.tmp", "Factory.generated.cs");
            }
        }

        [CanBeNull]
        private string GetSpecialType([NotNull] Type type)
        {
            if (type == typeof(IFactory))
            {
                return "this";
            }

            if (type == typeof(IConfiguration))
            {
                return "Configuration";
            }

            if (type == typeof(ICompositionService))
            {
                return "CompositionService";
            }

            if (type == typeof(ITraceService))
            {
                return "Trace";
            }

            if (type == typeof(IConsoleService))
            {
                return "Console";
            }

            if (type == typeof(IFileSystemService))
            {
                return "FileSystem";
            }

            return null;
        }

        [NotNull]
        protected string GetGenericTypeName([NotNull] Type type)
        {
            if (!type.GetTypeInfo().IsGenericType)
            {
                return GetTypeName(type);
            }

            var typeName = type.GetGenericTypeDefinition().FullName;
            typeName = typeName.Left(typeName.IndexOf('`'));

            var arguments = string.Join(", ", type.GetGenericArguments().Select(GetGenericTypeName));

            return typeName + "<" + arguments + ">";
        }

        [NotNull]
        protected string GetTypeName([NotNull] Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(float))
            {
                return "float";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (!type.GetTypeInfo().IsGenericType)
            {
                return type.FullName;
            }

            return GetGenericTypeName(type);
        }
    }
}
