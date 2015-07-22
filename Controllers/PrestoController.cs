
using System.Web.Mvc;
using System;
using System.Linq;
using System.Data.Spatial;
using Enyim.Collections;

namespace Presto.Controllers
{
  public class PrestoController : Controller
  {
    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Status(string service)
    {
        return Json(new {taxlotCount=Global.taxlots.Count()}, "application/json", JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Identify(string service)
    {
      double x, y;
      var is_x = Double.TryParse(Request.Params["x"], out x);
      var is_y = Double.TryParse(Request.Params["y"], out y);

      if (!is_x || !is_y)
      {
        return Json(new { error = true, message = "Please provide an x and y value in decimal degrees" }, JsonRequestBehavior.AllowGet);
      }

      var inPoint = DbGeometry.FromText("POINT("+x+" "+y+")", 4326);
      var env = new Envelope(x, y, x, y);

      var t = Global.r.Search(env);
      var res = t.Where(p => Global.taxlots[p.Data].Intersects(inPoint));
      var test = res.Count();

      if (test <= 0) return Json(new {x, y, error = true, message = "Nobody there"}, JsonRequestBehavior.AllowGet);

      var taxlotnum = res.FirstOrDefault().Data;

      return Json(new {TLID = taxlotnum, geom = Global.taxlots[taxlotnum].AsText()}, "application/json", JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Test()
    {
      var foo = DbGeometry.FromText(
        "POLYGON ((-122.66018778173442 45.601605211473235,-122.66024700952981 45.6014165334974,-122.6603231949731 45.601428354870642,-122.6602639784374 45.60161701917621,-122.66018778173442 45.601605211473235))");
      return Json(new { foo }, JsonRequestBehavior.AllowGet);
    }
  }
}