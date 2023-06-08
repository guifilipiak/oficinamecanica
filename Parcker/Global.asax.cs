using Parcker.App_Start;
using Parcker.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Parcker
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ModelBinders.Binders.Add(
                typeof(decimal), new BinderConfig());
            ModelBinders.Binders.Add(
                typeof(decimal?), new BinderConfig());

            AutoMapperConfig.RegisterMappings();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            
        }
    }
}
