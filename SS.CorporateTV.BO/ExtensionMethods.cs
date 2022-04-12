using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace SS.CorporateTV.BO.Web.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static IEnumerable<SelectListItem> ToSelectList(this Enum enumValue)
        {
            var elems = from Enum e in Enum.GetValues(enumValue.GetType()) select new { ID = e, Name = e.ToString() };
            return new SelectList(elems, "ID", "Name", enumValue);
        }

        public static MvcHtmlString ActionLinkWithIcon(this AjaxHelper helper, string iconclass, string title,
            string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            return ActionLinkWithIcon(helper, iconclass, title, actionName, routeValues, ajaxOptions, null);
        }



        public static string GetShortDateString(this DateTime? date)
        {
            return date.HasValue ? date.Value.ToShortDateString() : "";
        }


        public static MvcHtmlString ActionLinkWithIcon(this AjaxHelper helper, string iconclass, string title,
              string actionName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", iconclass);
            var link = helper.ActionLink("[substitui]", actionName, routeValues, ajaxOptions, htmlAttributes);
            return
                new MvcHtmlString(link.ToString()
                    .Replace("[substitui]",
                        String.Format("{0}</span>{1}", builder.ToString(TagRenderMode.StartTag), title)));
        }

        public static MvcHtmlString ActionLinkWithIcon(this AjaxHelper helper, string iconclass, string title,
            string actionName, string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var builder = new TagBuilder("span");
            builder.MergeAttribute("class", iconclass);
            var link = helper.ActionLink("[substitui]", actionName, controllerName, routeValues, ajaxOptions, htmlAttributes);
            return
                new MvcHtmlString(link.ToString()
                    .Replace("[substitui]",
                        String.Format("{0}</span>{1}", builder.ToString(TagRenderMode.StartTag), title)));
        }

        public static ActionResult CssFromView(this Controller controller, string cssViewName = null, object model = null)
        {
            var cssContent = ParseViewToContent(controller, cssViewName, "style", model);
            if (cssContent == null) throw new HttpException(404, "CSS not found");
            return new ContentResult() { Content = cssContent, ContentType = "text/css" };
        }

        static string ParseViewToContent(Controller controller, string viewName, string tagName, object model = null)
        {
            using (var viewContentWriter = new StringWriter())
            {
                if (model != null)
                    controller.ViewData.Model = model;

                if (string.IsNullOrEmpty(viewName))
                    viewName = controller.RouteData.GetRequiredString("action");

                var viewResult = new ViewResult()
                {
                    ViewName = viewName,
                    ViewData = controller.ViewData,
                    TempData = controller.TempData,
                    ViewEngineCollection = controller.ViewEngineCollection
                };

                var viewEngineResult = controller.ViewEngineCollection.FindPartialView(controller.ControllerContext, viewName);
                if (viewEngineResult.View == null)
                    return null;

                try
                {
                    var viewContext = new ViewContext(controller.ControllerContext, viewEngineResult.View, controller.ViewData, controller.TempData, viewContentWriter);
                    viewEngineResult.View.Render(viewContext, viewContentWriter);
                    var viewString = viewContentWriter.ToString().Trim('\r', '\n', ' ');
                    var regex = string.Format("<{0}[^>]*>(.*?)</{0}>", tagName);
                    var res = Regex.Match(viewString, regex, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Singleline);
                    if (res.Success && res.Groups.Count > 1)
                        return res.Groups[1].Value;
                    else throw new InvalidProgramException(string.Format("Dynamic content produced by viewResult '{0}' expected to be wrapped in '{1}' tag", viewName, tagName));
                }
                finally
                {
                    if (viewEngineResult.View != null)
                        viewEngineResult.ViewEngine.ReleaseView(controller.ControllerContext, viewEngineResult.View);
                }
            }

        }

    }

    public static class EnumHelper<T>
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;
            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }

    public static class CommonHtmlExtensions
    {
        public static object GetGlobalResource(this HtmlHelper htmlHelper, string classKey, string resourceKey)
        {
            //return htmlHelper.ViewContext.HttpContext.GetGlobalResourceObject(classKey, resourceKey);
            ResourceManager rm = new ResourceManager(classKey, typeof(CommonHtmlExtensions).Assembly);
            return rm.GetObject(resourceKey);
            //return Resources.HorarioBase.ResourceManager.GetObject(resourceKey);           
        }

        public static object GetGlobalResource(this HtmlHelper htmlHelper, string classKey, string resourceKey, CultureInfo culture)
        {
            return htmlHelper.ViewContext.HttpContext.GetGlobalResourceObject(classKey, resourceKey, culture);
        }

        public static object GetLocalResource(this HtmlHelper htmlHelper, string classKey, string resourceKey)
        {
            return htmlHelper.ViewContext.HttpContext.GetLocalResourceObject(classKey, resourceKey);
        }

        public static object GetLocalResource(this HtmlHelper htmlHelper, string classKey, string resourceKey, CultureInfo culture)
        {
            return htmlHelper.ViewContext.HttpContext.GetLocalResourceObject(classKey, resourceKey, culture);
        }

    }
}