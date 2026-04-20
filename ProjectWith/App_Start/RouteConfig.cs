//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using System.Web.Routing;

//namespace ProjectWith
//{
//    public class RouteConfig
//    {
//        public static void RegisterRoutes(RouteCollection routes)
//        {
//            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

//            routes.MapRoute(
//                name: "Default",
//                url: "{controller}/{action}/{id}",
//                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
//            );
//        }
//    }
//}


using System.Web.Mvc;
using System.Web.Routing;

namespace ProjectWith
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "MaFiche",
                url: "FicheFonctions/MaFiche",
                defaults: new { controller = "FicheFonctions", action = "MaFiche" } // Controller name adjusted
            );

            routes.MapRoute(
               name: "MesEmployes",
               url: "FicheFonctions/MesEmployes",
               defaults: new { controller = "FicheFonctions", action = "MesEmployes" }
           );


            // Default route (this should be at the bottom of your RegisterRoutes method)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
