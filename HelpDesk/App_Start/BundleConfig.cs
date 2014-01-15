using System.Web;
using System.Web.Optimization;

namespace HelpDesk
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));
            

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-switch.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/angularjs").Include(
                "~/scripts/angular/angular.js",
                "~/scripts/angular/angular-route.min.js",
                "~/scripts/angular/angular-cookies.min.js",
                "~/scripts/angular/angular-resource.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/appjs").Include(
                /*"~/scripts/angular/angular-ui-router.js",*/
                "~/scripts/jquery.pnotify.min.js",
                "~/scripts/select2.min.js",
                "~/Scripts/app/general/helpers.js",
                "~/Scripts/app/general/app.js",
                "~/Scripts/app/general/model_resources.js",
                "~/Scripts/app/components/services/filtering_service.js",
                "~/Scripts/app/components/services/paging_service.js",
                "~/Scripts/app/components/filter_component.js",
                "~/Scripts/app/components/pager_component.js",
                "~/Scripts/app/components/print_component.js",
                "~/Scripts/app/components/time_component.js",
                "~/Scripts/app/components/select2.js"));

            bundles.Add(new StyleBundle("~/Content/skins").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-switch.css",
                      "~/Content/jquery.pnotify.css",
                      "~/Content/jquery.pnotify.icons.css",
                      "~/Content/css/select2.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/animate.min.css",
                      "~/Content/app.css"));

        }
    }
}
