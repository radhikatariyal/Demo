using System;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Patient.Demographics.Configuration.Extensions
{
    public static class WindsorExtensions
    {
        public static Type[] GetPublicTypes(this Assembly assembly, Predicate<Type> where)
        {
            return assembly.GetExportedTypes()
                .Where(t => t.IsClass)
                .Where(t => t.IsAbstract == false)
                .Where(@where.Invoke)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        public static Type[] GetImplementationTypesFor<T>(this IWindsorContainer container)
        {
            return container.GetImplementationTypes(typeof(T));
        }


        public static Type[] GetImplementationTypes(this IWindsorContainer container, Type type)
        {
            return container.GetHandlers(type)
                .Select(h => h.ComponentModel.Implementation)
                .OrderBy(t => t.Name)
                .ToArray();
        }

        public static IHandler[] GetHandlers(this IWindsorContainer container, Type type)
        {
            return container.Kernel.GetAssignableHandlers(type);
        }

        public static IHandler[] GetHandlersFor<T>(this IWindsorContainer container)
        {
            return container.GetHandlers(typeof(T));
        }

        public static bool IsRegisteredAsService(this IHandler handler)
        {
            return handler.ComponentModel.Services.Single() != handler.ComponentModel.Implementation;
        }

        public static Type[] GetTypesFromAssemblySafe(this Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return new Type[] { };
            }
        }
    }
}