namespace Patient.Demographics.Service.FileUploads.Validators.ValidationChecks
{
    public class FieldValue
    {
        public FieldValue(string identifier, string field)
        {
            Identifier = identifier;
            Field = field;
        }
        public string Field { get;  }
        public string Identifier { get;  }
    }
}
