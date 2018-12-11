namespace Patient.Demographics.Common.Interfaces
{
    /// <summary>
    ///     Provides methods for mapping from one data class type to another/
    ///     For example mapping data models to domain models.
    /// if the implementation is marked with IDependency (or another dependency interface) as well as IAutomapperSetup, 
    ///  the implementation will be picked by the bootstrapper and the SetupMappings will be called after the container is built.
    /// </summary>
    public interface IAutoMapperSetup 
    {
        /// <summary>
        ///     Configures the mappings.
        /// </summary>
        void Start();
    }
}