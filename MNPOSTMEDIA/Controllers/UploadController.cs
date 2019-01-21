using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using MNPOSTMEDIA.Util;

namespace MNPOSTMEDIA.Controllers
{
    public class UploadController : Controller
    {


        [HttpPost]
        public ActionResult MailerImages()
        {
            var files = Request.Files;

            List<string> paths = new List<string>();
            
            if(files != null && files.Count > 0)
            {

                for(int i = 0; i < files.Count; i++)
                {
                    string dfolder =  DateTime.Now.Date.ToString("ddMMyyyy");

                    string fsave = "~/PIC/" + dfolder;

                    bool exists = System.IO.Directory.Exists(Server.MapPath(fsave));

                    if (!exists)
                        System.IO.Directory.CreateDirectory(Server.MapPath(fsave));
                    try
                    {
                        ImageUpload imageUpload = new ImageUpload()
                        {
                            isSacle = true,
                            Width = 1000,
                            UploadPath = fsave
                        };
                        MemoryStream target = new MemoryStream();
                        files[i].InputStream.CopyTo(target);
                        byte[] data = target.ToArray();

                        var imageResult = imageUpload.RenameUpload(data, ".png");

                        if (imageResult.Success)
                        {
                            paths.Add("/PIC/" + dfolder + "/" + imageResult.ImageName);
                        }

                    }
                    catch
                    {
                        return Json(new { id =  1 , msg = "Image upload to fail" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }

            return Json(new { id = 0, data = paths }, JsonRequestBehavior.AllowGet);
        }
    }
}