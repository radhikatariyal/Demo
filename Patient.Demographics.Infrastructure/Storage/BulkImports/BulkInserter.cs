using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Patient.Demographics.Common.Validation;
using Patient.Demographics.Data;

namespace Patient.Demographics.Infrastructure.Storage.BulkImports
{
    public interface IBulkInserter
    {
        void Insert(string destinationTableName);
        BulkInserter WithBatchSize(int batchSize);
        BulkInserter WithColumn(string sourceColumnName, string destinationColumnName, Type columnType);
        BulkInserter WithRowData(object[] rowData);
        BulkInserter WithProgressCallback(Func<int, Task> progressCallback, int numberOfInsertsToNotifyAfter);
    }

    public class BulkInserter : IBulkInserter
    {
        private const int DefaultBatchSize = 5000;

        private readonly SqlBulkCopy _sqlBulkCopy;
        private readonly DataTable _dataTable = new DataTable();
        private SqlRowsCopiedEventHandler _rowsCopiedEventHandler;

        public BulkInserter(ISqlDataContext dataContext)
        {
            _sqlBulkCopy = new SqlBulkCopy(dataContext.ConnectionString)
            {
                BatchSize = DefaultBatchSize
            };
        }

        public BulkInserter WithColumn(string sourceColumnName, string destinationColumnName, Type columnType)
        {
            ArgumentValidator.EnsureIsNotNullOrWhitespace(sourceColumnName, nameof(sourceColumnName));
            ArgumentValidator.EnsureIsNotNullOrWhitespace(destinationColumnName, nameof(destinationColumnName));
            ArgumentValidator.EnsureIsNotNull(columnType, nameof(columnType));

            _sqlBulkCopy.ColumnMappings.Add(sourceColumnName, destinationColumnName);
            _dataTable.Columns.Add(destinationColumnName, columnType);

            return this;
        }

        public BulkInserter WithRowData(object[] rowData)
        {
            ArgumentValidator.EnsureIsNotNull(rowData, nameof(rowData));

            _dataTable.Rows.Add(rowData);
            return this;
        }

        public BulkInserter WithBatchSize(int batchSize)
        {
            _sqlBulkCopy.BatchSize = batchSize;
            return this;
        }

        public BulkInserter WithProgressCallback(Func<int, Task> progressCallback, int numberOfInsertsToNotifyAfter)
        {
            ArgumentValidator.EnsureIsNotNull(progressCallback, nameof(progressCallback));

            _rowsCopiedEventHandler = async (sender, e) =>
            {
                await progressCallback((int)e.RowsCopied);
            };
            _sqlBulkCopy.NotifyAfter = numberOfInsertsToNotifyAfter;

            return this;
        }

        public void Insert(string destinationTableName)
        {
            ArgumentValidator.EnsureIsNotNullOrWhitespace(destinationTableName, nameof(destinationTableName));

            try
            {
                if (_rowsCopiedEventHandler != null)
                {
                    _sqlBulkCopy.SqlRowsCopied += _rowsCopiedEventHandler;
                }

                _sqlBulkCopy.DestinationTableName = destinationTableName;
                _sqlBulkCopy.WriteToServer(_dataTable);
            }
            finally
            {
                if (_rowsCopiedEventHandler != null)
                {
                    _sqlBulkCopy.SqlRowsCopied -= _rowsCopiedEventHandler;
                }
            }
        }
    }
}
