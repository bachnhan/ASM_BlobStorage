using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageStore.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return RedirectToAction("Upload");
        }

        public ActionResult Upload()
        {
            CloudBlobContainer blobContainer = BlobStorageService.GetCloudBlobContainer();
            List<string> blobs = new List<string>();
            blobContainer.ListBlobs().ToList().ForEach(s => blobs.Add(s.Uri.ToString()));
            return View(blobs);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase imageUpload)
        {
            if (imageUpload != null)
            {
                if (imageUpload.ContentLength <= 0 || imageUpload.ContentLength > (4 * 1024 * 1024))
                {
                    TempData["Msg"] = "File size is not valid.";
                    return RedirectToAction("Upload");
                }
                if (!string.Equals(imageUpload.ContentType, "image/jpg", StringComparison.OrdinalIgnoreCase)
                   && !string.Equals(imageUpload.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase)
                   && !string.Equals(imageUpload.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
                {
                    TempData["Msg"] = "File format is not valid.";
                    return RedirectToAction("Upload");
                }

                CloudBlobContainer blobContainer = BlobStorageService.GetCloudBlobContainer();
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(imageUpload.FileName);
                blob.UploadFromStream(imageUpload.InputStream);
            }
            else
            {
                TempData["Msg"] = "No file is selected.";
            }
            return RedirectToAction("Upload");
        }

        public ActionResult Delete(string imageName)
        {
            Uri uri = new Uri(imageName);
            string fileName = Path.GetFileName(uri.LocalPath);
            CloudBlobContainer blobContainer = BlobStorageService.GetCloudBlobContainer();
            CloudBlockBlob blob = blobContainer.GetBlockBlobReference(fileName);
            blob.Delete();
            TempData["Msg"] = "File " + fileName + " is deleted";
            return RedirectToAction("Upload");
        }
    }
}