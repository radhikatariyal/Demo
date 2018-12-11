using System.Reflection;
using Bootstrap;
using Bootstrap.Extensions.StartupTasks;
using Bootstrap.Windsor;

namespace Patient.Demographics.API
{
    public static class ApplicationManager
    {
        public static void Initialize(Assembly assembly)
        {
            Bootstrapper
                  .IncludingOnly
                      .Assembly(assembly)
                  .With
                      .Windsor().WithContainer(new WebApplicationContainer())
                  .With
                      .StartupTasks()
                  .Start();
        }
    }
}
