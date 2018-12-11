namespace Patient.Demographics.Common
{
    public class LinkTypes : Enumeration
    {
        public static readonly LinkTypes View = new LinkTypes("view");
        public static readonly LinkTypes Preview = new LinkTypes("preview");
        
        public LinkTypes()
        {
        }

        private LinkTypes(string name) : base(name)
        {
        }
    }
}
