
// Copyright Christophe Bertrand.

using System;
using System.Reflection;

namespace UniversalSerializerLib3.StreamFormat3
{
    /// <summary>
    /// Header of stream format, just after format version entry.
    /// </summary>
    public struct Header
    {
        /// <summary>
        /// Identifies assemblies that may contain modifiers (filters and containers).
        /// </summary>
        public AssemblyIdentifier[] AssemblyIdentifiers;
    }
    
    /// <summary>
    /// Identifies an assembly.
    /// </summary>
    public struct AssemblyIdentifier
    {
        /// <summary>
        /// Got from {Assembly}.GetName().Name .
        /// To be used by Assembly.LoadWithPartialName(PartialName).
        /// </summary>
        public string PartialName;

        /// <summary>
        /// Got from {Assembly}.Location .
        /// To be used by Assembly.LoadFrom(Location).
        /// </summary>
        public string Location;

        /// <summary>
        /// Got from {Assembly}.GetName().FullName .
        /// To be used by Assembly.Load(new AssemblyName(AssemblyName_FullName)).
        /// </summary>
        public string AssemblyNameFullName;

        internal AssemblyIdentifier(Assembly a)
        {
            var an = a.GetName();
            this.PartialName = an.Name;
            this.AssemblyNameFullName = an.FullName;
            this.Location = a.Location;
        }

        static readonly SimpleLazy<string> ThisAssemblyName =
            new SimpleLazy<string>(() => Assembly.GetExecutingAssembly().GetName().FullName);

        internal Assembly Load()
        {
            if (ThisAssemblyName.Value == this.AssemblyNameFullName)
                return Assembly.GetExecutingAssembly();
            try
            {
                return Assembly.LoadFrom(this.Location);
            }
            catch (Exception e)
            {
                Log.WriteLine(string.Format(
                    ErrorMessages.GetText(17) //"Listed assembly in stream can not be loaded: \"{0}\". Error={1}"
                    , this.AssemblyNameFullName, e.Message));
                return null;
            }
        }
    }
}