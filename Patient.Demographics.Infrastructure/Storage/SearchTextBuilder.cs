using System.Text;
using Patient.Demographics.Domain;

namespace Patient.Demographics.Infrastructure.Storage
{
    public static class SearchTextBuilder
    {
        public static void AddItemToSearchText(StringBuilder searchText, string value)
        {
            searchText.Append($"\"{value}");
        }
    }
}