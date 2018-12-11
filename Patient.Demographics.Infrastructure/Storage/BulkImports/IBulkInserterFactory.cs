namespace Patient.Demographics.Infrastructure.Storage.BulkImports
{
    public interface IBulkInserterFactory
    {
        IBulkInserter Create();
        void Release(IBulkInserter bulkInserter);
    }
}
