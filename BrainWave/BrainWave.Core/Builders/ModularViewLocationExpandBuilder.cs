using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.Razor
{

    public class ModularViewLocationExpandBuilder : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            var result = new List<string>();
            result.AddRange(viewLocations);
            var extensionViewsPath = "/Packages/" + context.AreaName + "/Views";
            result.Add(extensionViewsPath + "/{1}/{0}" + RazorViewEngine.ViewExtension);
            result.Add(extensionViewsPath + "/Shared/{0}" + RazorViewEngine.ViewExtension);
            return result;
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
