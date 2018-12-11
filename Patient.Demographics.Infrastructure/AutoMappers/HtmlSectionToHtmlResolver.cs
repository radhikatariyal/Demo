using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Patient.Demographics.Common.Interfaces;
using Patient.Demographics.CrossCutting.Security;
using HtmlAgilityPack;

namespace Patient.Demographics.Infrastructure.AutoMappers
{
    public class HtmlSectionToHtmlResolver : IMemberValueResolver<ILocalisedSectionContainer, ILocalisedContentView, IEnumerable<ILocalisedSection>, string>
    {
        /// <summary>
        /// The mapping code is responsible for assigning the HTML string and the Audience / Permission id
        /// using BeforeMap method as below:
        /// <code>
        /// CreateMap&lt;ReportPageEntity, ReportPageViewDto&gt;()
        ///     .BeforeMap((entity, dto, ctx) =>
        ///        {
        ///            ctx.Items["ContentTemplate"] = entity.ContentTemplate.Html;
        ///            ctx.Items["ReportingAudienceId"] = entity.Promotion.ReportingAudienceId;
        ///       })
        /// </code>
        /// The calling code then only needs to specify the culture for mapping as below:
        /// <code>
        ///     Mapper.Map&lt;ReportPageViewDto&gt;(reportPage, options =&gt; { options.Items["culture"] = culture; });</code>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceMember"></param>
        /// <param name="destinationMember"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string Resolve(ILocalisedSectionContainer source, ILocalisedContentView destination, IEnumerable<ILocalisedSection> sourceMember, string destinationMember, ResolutionContext context)
        {
            var targetCulture = context.Items["culture"].ToString();
            var template = context.Items["ContentTemplate"].ToString();
            var audienceId = (Guid)context.Items["ReportingAudienceId"];
            bool? previewOnly = false;
            if (context.Items.ContainsKey("previewOnly"))
                previewOnly = context.Items["previewOnly"] as bool?;
            var htmlContent = string.Empty;
            var sections =
                sourceMember.Where(sectionEntity => sectionEntity.Culture == targetCulture)
                    .OrderBy(x => x.SectionId)
                    .ToList();

            if (sections.All(s => !string.IsNullOrEmpty(s.Content)))
            {
                htmlContent = AssembleFullTextContent(template, sections, audienceId, previewOnly??false);
            }
            return htmlContent;
        }

        private static string AssembleFullTextContent(string templateHtml,
            IEnumerable<ILocalisedSection> sections, Guid audienceId, bool previewOnly)
        {
            const string sectionPlaceholder = "<nx-content-editor/>";
            var result = templateHtml;
            // take the query parameter, permissions id and encrypt it
            // then add http-src="encrypted token"
            // if preview mode set mode attribute to preview othervise view
            foreach (var section in sections)
            {
                var placeholderIndex = result.IndexOf(sectionPlaceholder, StringComparison.OrdinalIgnoreCase);
                if (placeholderIndex > 0)
                {
                    result = result.Remove(placeholderIndex, sectionPlaceholder.Length)
                        .Insert(placeholderIndex, section.Content);
                }
            }
            var document = new HtmlDocument();
            document.LoadHtml(result);
            document.OptionOutputAsXml = true;
            var root = document.DocumentNode;
            ProcessImages(audienceId, previewOnly, root);
            ProcessAnchors(audienceId, previewOnly, root);
            return HtmlEntity.DeEntitize(document.DocumentNode.InnerHtml);
        }


        private static void ProcessImages(Guid audienceId, bool previewOnly, HtmlNode root)
        {
            var imageElements = root.Descendants("img");
            const string imageRelativeUrlSplitToken = "?path=";
            const string imageTokenSplitToken = "&amp;token=";
            foreach (var htmlNode in imageElements)
            {
                string imageSource = htmlNode.Attributes["src"].Value;
                //only keep the image path parameter
                int index = imageSource.IndexOf(imageRelativeUrlSplitToken, StringComparison.OrdinalIgnoreCase) + imageRelativeUrlSplitToken.Length;
                imageSource = imageSource.Substring(index);
                if (imageSource.IndexOf("token=", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    index = imageSource.IndexOf(imageTokenSplitToken, StringComparison.OrdinalIgnoreCase);
                    imageSource = imageSource.Substring(0, index);
                }
                //the images will be loaded via http-src so clear the src attribute
                var token = new EncryptionDecryption().Encrypt($"audienceId={audienceId}&fileUrl={imageSource}");
                htmlNode.SetAttributeValue("http-src", token);
                htmlNode.SetAttributeValue("src", "");
                htmlNode.Attributes.Remove("data-original-src");
                htmlNode.SetAttributeValue("http-src-type", previewOnly ? "preview" : "view");
            }
        }

        private static void ProcessAnchors(Guid permissionsId, bool previewOnly, HtmlNode root)
        {
            var imageElements = root.Descendants("a");

            const string imageRelativeUrlSplitToken = "?path=";
            const string tokenUrlPosition = "token=";

            foreach (var htmlNode in imageElements)
            {
                string htref = htmlNode.Attributes["href"].Value;
                //only keep the image path parameter

                Uri parsedUri;

                if (Uri.TryCreate(htref, UriKind.RelativeOrAbsolute, out parsedUri) && htref.IndexOf(tokenUrlPosition, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var splitTokenPosition = htref.IndexOf(imageRelativeUrlSplitToken, StringComparison.OrdinalIgnoreCase);

                    if (splitTokenPosition >= 0)
                    {
                        int index = htref.IndexOf(imageRelativeUrlSplitToken, StringComparison.OrdinalIgnoreCase) + imageRelativeUrlSplitToken.Length;
                        htref = htref.Substring(index);
                    }

                    //the images will be loaded via http-src so clear the src attribute
                    var token = new EncryptionDecryption().Encrypt($"permissionId={permissionsId}&fileUrl={htref}");
                    htmlNode.SetAttributeValue("nx-http-href", token);
                    htmlNode.SetAttributeValue("href", "");
                    htmlNode.SetAttributeValue("nx-http-href-type", previewOnly ? "preview" : "view");
                }
            }
        }
    }
}