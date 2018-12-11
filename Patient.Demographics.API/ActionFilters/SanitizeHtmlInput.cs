using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Patient.Demographics.API.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SanitizeHtmlInput : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (actionContext.Request.Method.Method.ToUpper() != "GET"
                && actionContext.Request.Method.Method.ToUpper() != "OPTIONS"
                && actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                if (actionContext.ActionArguments != null && (actionContext.ActionArguments.Count == 1 || actionContext.ActionArguments.Count == 2))
                {
                    KeyValuePair<string, object> requestParam;

                    if (actionContext.ActionArguments.Count == 1)
                    {
                        requestParam = actionContext.ActionArguments.First();
                    }
                    else
                    {
                        requestParam = actionContext.ActionArguments.Last();
                    }

                    if (requestParam.Value != null)
                    {
                        var properties = requestParam.Value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(string) && x.GetGetMethod(true).IsPublic && x.GetSetMethod(true).IsPublic);

                        if (properties.Any())
                        {
                            foreach (var propertyInfo in properties)
                            {
                                var data = propertyInfo.GetCustomAttributesData();
                                var requiredFlag = false;
                                var maxLengthFlag = false;
                                var maxLength = 0;

                                var valueAsString = propertyInfo.GetValue(requestParam.Value) as string;

                                if (!string.IsNullOrWhiteSpace(valueAsString))
                                {

                                    if (data.Any())
                                    {
                                        var dataTypes = data.Where(x => x.AttributeType == typeof(DataTypeAttribute));

                                        if (dataTypes.Any())
                                        {
                                            if (dataTypes.Any(x => x.ConstructorArguments.Any(y => y.Value.ToString() == ((int)DataType.Html).ToString())))
                                            {
                                                break;
                                            }
                                        }

                                        requiredFlag = data.Any(x => x.AttributeType == typeof(RequiredAttribute));

                                        maxLengthFlag = data.Any(x => x.AttributeType == typeof(MaxLengthAttribute));

                                        if (maxLengthFlag)
                                        {
                                            maxLength = Convert.ToInt32(data.First(x => x.AttributeType == typeof(MaxLengthAttribute)).ConstructorArguments.First().Value);
                                        }
                                    }

                                    var sanitizer = new HtmlSanitizer();

                                    var allowedTags = sanitizer.AllowedTags.ToList();

                                    foreach (var tag in allowedTags)
                                    {
                                        sanitizer.AllowedTags.Remove(tag);
                                    }

                                    sanitizer.AllowedTags.Add("&");
                                    sanitizer.AllowedTags.Add("<");
                                    sanitizer.AllowedTags.Add(">");

                                    var allowedAttributes = sanitizer.AllowedAttributes.ToList();

                                    foreach (var attribute in allowedAttributes)
                                    {
                                        sanitizer.AllowedAttributes.Remove(attribute);
                                    }

                                    var allowedCssProperties = sanitizer.AllowedCssProperties.ToList();

                                    foreach (var cssProperty in allowedCssProperties)
                                    {
                                        sanitizer.AllowedCssProperties.Remove(cssProperty);
                                    }

                                    sanitizer.AllowedSchemes.Add("mailto");

                                    var sanitizedInput = sanitizer.Sanitize(valueAsString);

                                    sanitizedInput = sanitizedInput.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");

                                    if (string.IsNullOrEmpty(sanitizedInput) && requiredFlag)
                                    {
                                        actionContext.ModelState.AddModelError(string.Empty, string.Format("{0} is a required field and your input has been invalidated as it contained HTML.", propertyInfo.Name));
                                    }

                                    if (maxLengthFlag && sanitizedInput.Length > maxLength)
                                    {
                                        actionContext.ModelState.AddModelError(string.Empty, string.Format("{0} has HTML characters that when converted mean the content for the string is now too long. The max length is {1}.", propertyInfo.Name, maxLength));
                                    }

                                    propertyInfo.SetValue(requestParam.Value, string.IsNullOrEmpty(sanitizedInput) ? null : sanitizedInput);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
