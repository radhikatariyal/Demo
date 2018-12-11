namespace Patient.Demographics.Data
{
    public interface ISqlDataContextFactory
    {
        ISqlDataContext Create();

        void Release(ISqlDataContext sqlDataContext);
    }
}