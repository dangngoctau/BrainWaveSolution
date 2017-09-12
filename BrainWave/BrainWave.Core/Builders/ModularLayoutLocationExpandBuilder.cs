namespace Microsoft.AspNetCore.Mvc.Razor
{
    public static class ModularLayoutLocationExpandBuilder
    {
        public static RazorViewEngineOptions AddModularLayoutLocationFormats(this RazorViewEngineOptions options)
        {
            // todo: move Packages, DefaultTheme to config.
            options.AreaViewLocationFormats.Clear();
            options.AreaViewLocationFormats.Add("/Packages/DefaultTheme/{2}/{1}/{0}" + RazorViewEngine.ViewExtension);
            options.AreaViewLocationFormats.Add("/Packages/DefaultTheme/Views/" + "{0}" + RazorViewEngine.ViewExtension);

            return options;
        }
    }


}
