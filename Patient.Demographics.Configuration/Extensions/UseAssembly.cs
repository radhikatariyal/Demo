using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Patient.Demographics.Configuration.Extensions
{
    public static class UseAssembly
    {
        public static Assembly This()
        {
            return Assembly.GetCallingAssembly();
        }

        public static Assembly Containing<T>()
        {
            return typeof(T).Assembly;
        }

        public static IEnumerable<Assembly> All()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Assembly> AllExcludingKnownThirdPartyAssemblies()
        {
            return All().Where(assembly => !IsKnownThirdPartyAssembly(assembly));
        }

        private static bool IsKnownThirdPartyAssembly(Assembly assembly)
        {
            var companyAttribute = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
            var productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            
            return IsKnownThirdPartyAssembly(companyAttribute) || IsKnownThirdPartyAssembly(productAttribute) || IsTemporaryAssembly(assembly);
        }

        private static bool IsKnownThirdPartyAssembly(AssemblyCompanyAttribute companyAttribute)
        {
            if (companyAttribute == null)
            {
                return false;
            }

            var companiesToExclude = new List<string>
            {
                "Microsoft", "Castle", "Quartz", "Newtonsoft", "Ploeh"
            };

            return companiesToExclude.Any(companyToExclude => companyAttribute.Company?.IndexOf(companyToExclude, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool IsKnownThirdPartyAssembly(AssemblyProductAttribute productAttribute)
        {
            if (productAttribute == null)
            {
                return false;
            }

            var productsToExclude = new List<string>
            {
                "Topshelf", "MassTransit", "Common Logging Framework", "CsvHelper", "RabbitMQ", "EntityFramework", "NewId",
                "IdentityModel", "Fluent Assertions", "NSubstitute", "xUnit"
            };

            return productsToExclude.Any(productToExclude => productAttribute.Product?.IndexOf(productToExclude, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool IsTemporaryAssembly(Assembly assembly)
        {
            return assembly.FullName.IndexOf("DynamicProxyGenAssembly2", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static FromAssemblyDescriptor InApplicationDirectory(string assemblyNamePrefix = "")
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;

            var uri = new UriBuilder(codeBase);

            var path = Uri.UnescapeDataString(uri.Path);

            var dir = Path.GetDirectoryName(path);
            return Classes.FromAssemblyInDirectory(new AssemblyFilter(dir).FilterByAssembly(c =>
                                                                                            c.FullName.StartsWith(assemblyNamePrefix) && 
                                                                                            !c.FullName.StartsWith("xunit") && 
                                                                                            !c.GlobalAssemblyCache));
        }
    }
}