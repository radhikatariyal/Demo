using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Patient.Demographics.Configuration.Extensions
{
    public class AssemblyFilterBuilder
    {
        private readonly string _directoryName;
        private bool _excludeKnownThirdPartyAssemblies;

        private AssemblyFilterBuilder(string directoryName)
        {
            _directoryName = directoryName;
        }

        public static AssemblyFilterBuilder ForCurrentDirectory()
        {
            return ForDirectory(".");
        }

        public static AssemblyFilterBuilder ForDirectory(string directoryName)
        {
            return new AssemblyFilterBuilder(directoryName);
        }

        public AssemblyFilterBuilder ExcludeKnownThirdPartyAssemblies()
        {
            _excludeKnownThirdPartyAssemblies = true;
            return this;
        }

        public AssemblyFilter Build()
        {
            var assemblyFilter = new AssemblyFilter(_directoryName);

            if (_excludeKnownThirdPartyAssemblies)
            {
                assemblyFilter = ExcludeKnownThirdPartyAssemblies(assemblyFilter);
            }

            return assemblyFilter;
        }

        private static AssemblyFilter ExcludeKnownThirdPartyAssemblies(AssemblyFilter assemblyFilter)
        {
            var namesToExclude = new List<string>
            {
                "Microsoft", "Castle", "Quartz", "Newtonsoft", "Ploeh", "Topshelf", "MassTransit", "Common Logging Framework",
                "CsvHelper", "RabbitMQ", "EntityFramework", "NewId", "IdentityModel", "Fluent Assertions", "NSubstitute", "xUnit"
            };

            return assemblyFilter.FilterByName(assembly => namesToExclude.All(nameToExclude => !CaseInsensitiveContains(assembly.FullName, nameToExclude)));
        }

        private static bool CaseInsensitiveContains(string source, string value)
        {
            return source.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}
