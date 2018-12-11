using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;

namespace Patient.Demographics.Configuration.Extensions
{
    public static class InstallerExtensions
    {
        public static IWindsorInstaller[] FindInstallers(this Assembly assembly)
        {
            try
            {
                var installers = (from type in assembly.GetTypes()
                                  where typeof(IWindsorInstaller).IsAssignableFrom(type)
                                  select Activator.CreateInstance(type) as IWindsorInstaller).ToArray();

                return installers;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static IWindsorInstaller[] Exclude<T>(this IWindsorInstaller[] installers) where T : IWindsorInstaller
        {
            return installers.Where(installer => !(installer is T)).ToArray();
        }

        public static IWindsorInstaller[] OrderFirst<T>(this IWindsorInstaller[] installers) where T : IWindsorInstaller
        {
            var newOrder = installers.OrderBy(installer => !(installer is T)).ToArray();

            return newOrder;
        }
    }
}