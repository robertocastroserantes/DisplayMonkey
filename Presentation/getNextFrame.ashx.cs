/*!
* DisplayMonkey source file
* http://displaymonkey.org
*
* Copyright (c) 2015 Fuel9 LLC and contributors
*
* Released under the MIT license:
* http://opensource.org/licenses/MIT
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using DisplayMonkey.Language;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DisplayMonkey
{
    public partial class getNextFrame : HttpTaskAsyncHandler
    {
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;

            int panelId = Request.IntOrZero("panel");
            int displayId = Request.IntOrZero("display");
            int frameId = Request.IntOrZero("frame");
            string culture = Request.StringOrBlank("culture");
			string json = "";
				
			try
			{
                // set culture
                if (!string.IsNullOrWhiteSpace(culture))
                {
                    System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(culture);
                    System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
                    System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;
                }

                JavaScriptSerializer s = new JavaScriptSerializer();
                Frame nci = await Frame.GetNextFrameAsync(panelId, displayId, frameId);
                json = s.Serialize(nci);
			}

			catch (Exception ex)
			{
                JavaScriptSerializer s = new JavaScriptSerializer();
                json = s.Serialize(new
                {
                    Error = ex.Message,
                    //Stack = ex.StackTrace,
                    Data = new
                    {
                        FrameId = frameId,
                        PanelId = panelId,
                        DisplayId = displayId,
                    },
                });
            }

            Response.Clear();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetSlidingExpiration(true);
            Response.Cache.SetNoStore();
            Response.ContentType = "application/json";
            Response.Write(json);
            Response.Flush();
        }
    }
}