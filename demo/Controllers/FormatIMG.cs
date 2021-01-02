using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.Web.Compilation;
using System.Web.Mvc;

namespace demo.Controllers
{
    public static class FormatIMG
    {
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText, string height, string width)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);
            builder.MergeAttribute("width", width);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText, string height, string width,string radius)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", src);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);

            builder.MergeAttribute("width", width);
            builder.MergeAttribute("style","border-radius:"+radius);
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.SelfClosing));
        }
    }
}