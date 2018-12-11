using Patient.Demographics.Common;
using System.Collections.Generic;

namespace Patient.Demographics.Service.FileUploads.Importers
{
    public abstract class ImportPersistanceBase
    {
        protected static string GetValueFromAttributes<T>(IDictionary<string, RowAttribute> attributes, T columnName) where T : Enumeration
        {
            return GetValueFromAttributes(attributes, columnName.Name);
        }

        protected static string GetValueFromAttributes(IDictionary<string, RowAttribute> attributes, string columnName)
        {
            // TODO: Can we remove the Trim() call? [LH]
            return attributes.ContainsKey(columnName) ? attributes[columnName].ValueAsString.Trim() : "";
        }
    }
}