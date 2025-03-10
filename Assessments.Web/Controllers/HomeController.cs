using System.Xml;
using Assessments.Web.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Assessments.Web.Controllers
{
    public class HomeController : BaseController<HomeController>
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [Route("sitemap")]
        public async Task<FileResult> SiteMap()
        {
            var path = Path.Combine(Environment.ContentRootPath, Constants.CacheFolder, "sitemap.xml");
            if (!System.IO.File.Exists(path))
            {
                var basepath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.PathBase.Value.ToString()}{this.Request.Path.Value.ToString()}".Replace("sitemap", "");
                var query = await DataRepository.GetSpeciesAssessments();
                var items = query.Select(x => x.Id);
                XmlWriterSettings xws = new XmlWriterSettings { Indent = true };
                XmlWriter xw = XmlWriter.Create(path, xws);

                xw.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                // Application root/index

                xw.WriteStartElement("url");
                xw.WriteElementString("loc", basepath + "rodlisteforarter/2021/");
                xw.WriteEndElement();

                foreach (var assessment in items)
                {
                    xw.WriteStartElement("url");
                    xw.WriteElementString("loc",
                        basepath + "rodlisteforarter/2021/" + assessment);
                    // xw.WriteElementString("lastmod", assessment.Updated); // No timestamp for assessment?
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.Flush();
                xw.Close();
            }

            //var virtualPath = Path.Combine(Constants.CacheFolder, "sitemap.xml");
            return File(System.IO.File.ReadAllBytes(path), "application/xhtml+xml", "sitemap.xml");
        }

    }
}
