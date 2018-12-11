using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public interface IRowValidationCheck
    {
        Task PerformCheck(ICollection<string> errors, FieldCollection fieldCollection);
    }

    public class FieldCollection
    {
        private readonly Dictionary<string, string> _fields;

        public const string IgnoredColumnName = "__Ignored";

        public FieldCollection()
        {
            _fields = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void AddField(string destinationColumnName, string fieldAsString)
        {
            _fields.Add(destinationColumnName, fieldAsString);
        }

        public bool IsMapped(string destinationColumnName)
        {
            return _fields.ContainsKey(destinationColumnName);
        }

        public string this[string destinationColumnName] => _fields.ContainsKey(destinationColumnName) ? _fields[destinationColumnName] : null;
    }
}