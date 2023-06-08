using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parcker.Extension
{
    public static class AttributesExtension
    {
        public static RouteValueDictionary IsDisabled(this object htmlAttributes, bool disabled)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            if (disabled)
            {
                if (!attributes.Any(x => x.Key == "disabled"))
                    attributes.Add("disabled", "disabled");
            }
            else
            {
                if (attributes.Any(x => x.Key == "disabled"))
                    attributes.Remove("disabled");
            }

            return attributes;
        }

        public static RouteValueDictionary IsReadonly(this object htmlAttributes, bool @readonly)
        {
            var attributes = new RouteValueDictionary(htmlAttributes);
            if (@readonly)
            {
                if (!attributes.Any(x => x.Key == "readonly"))
                    attributes.Add("readonly", "readonly");
            }
            else
            {
                if (attributes.Any(x => x.Key == "readonly"))
                    attributes.Remove("readonly");
            }

            return attributes;
        }
    }
}