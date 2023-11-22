using System.Web.Optimization;


namespace WUFT.NET
{
    public class BundleConfig
        {
            // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
            public static void RegisterBundles(BundleCollection bundles)
            {
                //bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                //        "~/Scripts/jquery-{version}.js"
                //        ));

                //bundles.Add(new Bundle("~/bundles/jqueryval").Include(
                //        "~/Scripts/jquery.validate.js",
                //        "~/Scripts/jquery.validate.unobtrusive.js"
                //        ));


                bundles.UseCdn = true;

                bundles.Add(new ScriptBundle("~/bundles/jquery", @"http://isecdn.intel.com/jquery-2.1.1.js").Include(
                            "~/Scripts/jquery-{version}.js"));

                bundles.Add(new ScriptBundle("~/bundles/jqueryval", @"http://isecdn.intel.com/jquery.validate.js").Include(
                            "~/Scripts/jquery.validate*",
                            "~/Scripts/jquery.unobtrusive-ajax.js"));

                //// Use the development version of Modernizr to develop with and learn from. Then, when you're
                //// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
                bundles.Add(new ScriptBundle("~/bundles/modernizr", @"http://isecdn.intel.com/modernizr-2.6.2.js").Include(
                            "~/Scripts/modernizr-*"));

                bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                          "~/Scripts/bootstrap.js",
                          "~/Scripts/_references.js",
                          "~/Scripts/bootstrap-multiselect.js",
                          "~/Content/plugins/datepicker/*.js",
                          "~/Content/plugins/alertify/*.js",
                          "~/Content/plugins/modal/*.js",
                          "~/Content/plugins/selectize/*.js"
                          )
                          );

                bundles.Add(new ScriptBundle("~/bundles/wuft").Include(
                    "~/Scripts/list.js",
                    "~/Scripts/common.js",    
                    "~/Scripts/qre.js",
                    "~/Scripts/operator.js",
                    "~/Scripts/report.js",
                    "~/Scripts/admin.js"
                    ));

                bundles.Add(new StyleBundle("~/content/bundled-styles").Include(
                    "~/Content/bootstrap-theme.css",
                    "~/Content/bootstrap.css",
                    "~/Content/bootstrap-multiselect.css",
                    "~/Content/plugins/datepicker/*.css",
                    "~/Content/plugins/alertify/*.css",
                    "~/Content/plugins/modal/*.css",
                    "~/Content/plugins/selectize/*.css",
                    "~/Content/site/brand.css",
                    "~/Content/site/common.css",
                    "~/Content/site/helpers.css",
                    "~/Content/Site.css",
                    "~/Content/loading.css"
                )
                .Include("~/Content/site/layout.css", new CssRewriteUrlTransform()
                ));

                // Set EnableOptimizations to false for debugging. For more information,
                // visit http://go.microsoft.com/fwlink/?LinkId=301862
                BundleTable.EnableOptimizations = false;
            }
        }
}
