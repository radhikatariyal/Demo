using System;
using AutoMapper;
using Patient.Demographics.Common;
using Patient.Demographics.Common.Interfaces;
using Patient.Demographics.CrossCutting.Security;
using HtmlAgilityPack;

namespace Patient.Demographics.Infrastructure.AutoMappers
{
    public class AdminSectionHtmlTokenRefreshResolver :
        IMemberValueResolver<ILocalisedSection, ILocalisedSection, string, string>
    {
        public string Resolve(ILocalisedSection source, ILocalisedSection destination,
            string sourceMember, string destinationMember, ResolutionContext context)
        {
            var html = UpdateImageTokensForAdmin(sourceMember);
            return html;
        }

        private static string UpdateImageTokensForAdmin(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            document.OptionOutputAsXml = true;
            var root = document.DocumentNode;

            var imageElements = root.Descendants("img");
            const string queryParameterSplitter = "?path=";
            const string tokenSplitter = "&token=";

            foreach (var htmlNode in imageElements)
            {
                var imageSource = HtmlEntity.DeEntitize(htmlNode.Attributes["src"].Value);
                //only keep the image path parameter
                var parameterstartIndex = imageSource.IndexOf(queryParameterSplitter, StringComparison.OrdinalIgnoreCase);

                var baseUrl = imageSource.Substring(0, parameterstartIndex);
                var queryParameter = imageSource.Substring(parameterstartIndex + queryParameterSplitter.Length);
                var tokenIndex = queryParameter.IndexOf(tokenSplitter, StringComparison.OrdinalIgnoreCase);

                string imagePath;
                if (tokenIndex >= queryParameterSplitter.Length)
                {
                    imagePath = queryParameter.Substring(0, tokenIndex);
                }
                else
                {
                    imagePath = queryParameter;
                }

                //the images will be loaded via http-src so clear the src attribute
              //  var expiryDate = SystemDate.NowOffset().AddMinutes(120);
                //var token =
                //    new EncryptionDecryption().Encrypt($"fileUrl={imagePath}&expiryUtc={expiryDate.UtcTicks}");

               // htmlNode.SetAttributeValue("src", baseUrl + $"?path={imagePath}&amp;token=" + token);
            }
            return HtmlEntity.DeEntitize(document.DocumentNode.InnerHtml);
        }
    }
}