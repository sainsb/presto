using System;
using System.Collections.Generic;
using System.Data.Spatial;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Enyim.Collections;

namespace Presto
{

    public class Global : HttpApplication
    {

      internal static Dictionary<string, DbGeometry> taxlots;
      internal static RTree<string> r;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
               "Status", // Route name
               "status/{service}", // URL with parameters
               new { controller = "Presto", action = "Status" } // Parameter defaults
           );

            routes.MapRoute(
                 "Identify", // Route name
                 "identify/{service}", // URL with parameters
                 new { controller = "Presto", action = "Identify" } // Parameter defaults
             );

            routes.MapRoute(
                 "Test", // Route name
                 "test/", // URL with parameters
                 new { controller = "Presto", action = "Test" } // Parameter defaults
             );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);

            taxlots = new Dictionary<string, DbGeometry>();

            var lines = System.IO.File.ReadAllLines(@"c:/temp/tracts2.csv");

            r = new RTree<string>();

            foreach (var line in lines)
            {

              if (line.IndexOf('"') == -1) continue;

              var str_geom = line.Substring(1, line.IndexOf('"', 2)).Replace("\"", "");
              var TLID = "";
              var _geom = DbGeometry.FromText(str_geom, 4326);
              var _attr = line.Substring(line.IndexOf('"', 2), line.Length - line.IndexOf('"', 2));
              try
              {
                TLID = _attr.Replace("\",", "");
                Global.taxlots.Add(TLID, _geom);
                //attr.Add(Convert.ToInt32(DissolveID), _attr.Replace(DissolveID, "").Replace(",","").Replace("\"",""));
                var _boundary = _geom.Envelope;

                var env = new Envelope((double)_boundary.PointAt(1).XCoordinate, (double)_boundary.PointAt(1).YCoordinate,
                  (double)_boundary.PointAt(3).XCoordinate, (double)_boundary.PointAt(3).YCoordinate);

                r.Insert(TLID, env);
              }
              catch (Exception ex)
              {
              }
            }
        }
    }
}