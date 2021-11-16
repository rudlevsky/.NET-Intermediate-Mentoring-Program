using ProfileSample.DAL;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace ProfileSample.Controllers
{
	public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var context = new ProfileSampleEntities();
            var imgSources = new List<string>();
            var sources = context.ImgSources.Take(20).Select(x => x.Data);

			foreach (var bytes in sources)
			{
				var converted = System.Convert.ToBase64String(bytes);
                imgSources.Add(string.Format("data:image/jpg;base64,{0}", converted));
            }

			return View(imgSources);
        }

        public ActionResult Convert()
        {
            var files = Directory.GetFiles(Server.MapPath("~/Content/Img"), "*.jpg");

            using (var context = new ProfileSampleEntities())
            {
                foreach (var file in files)
                {
                    using (var stream = new FileStream(file, FileMode.Open))
					{
                        byte[] buff = new byte[stream.Length];

                        stream.Read(buff, 0, (int)stream.Length);

                        var entity = new ImgSource
                        {
                            Name = Path.GetFileName(file),
                            Data = buff,
                        };

                        context.ImgSources.Add(entity);
                        context.SaveChanges();
                    }
				} 
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
    }
}