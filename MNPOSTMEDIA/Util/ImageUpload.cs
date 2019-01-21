using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MNPOSTMEDIA.Util
{
    public class ImageUpload
    {
        // set default size here
        public int Width { get; set; }

        public int Height { get; set; }

        public bool isSacle = false;

        // folder for the upload, you can put this in the web.config
        public string UploadPath { get; set; }

     
        public ImageResult RenameUpload(byte[] image, string extension, Int32 counter = 0)
        {
            var fileName = DateTime.Now.ToString("ddMMyyyyhhmmss");

            string finalFileName = fileName + extension;

            if (counter != 0)
            {
                finalFileName = fileName + "_" + ((counter).ToString()) + extension;
            }
            if (System.IO.File.Exists
                (HttpContext.Current.Request.MapPath(UploadPath + "/" + finalFileName)))
            {
                //file exists => add country try again
                return RenameUpload(image, extension, ++counter);
            }
            //file doesn't exist, upload item but validate first
            return UploadFile(image, finalFileName, extension);
        }


        public ImageResult RenameUploadAvatar(byte[] image, string extension, string fileName)
        {
            var finalFileName = fileName + extension;

            if (System.IO.File.Exists
                (HttpContext.Current.Request.MapPath(UploadPath + "/" + finalFileName)))
            {
                System.IO.File.Delete(HttpContext.Current.Request.MapPath(UploadPath + "/" + finalFileName));
            }
            return UploadFile(image, finalFileName, extension);
        }

        private ImageResult UploadFile(byte[] file, string fileName, string extension)
        {
            ImageResult imageResult = new ImageResult { Success = true, ErrorMessage = null };

            var path =
          Path.Combine(HttpContext.Current.Request.MapPath(UploadPath), fileName);

            //make sure the file is valid
            if (!ValidateExtension(extension))
            {
                imageResult.Success = false;
                imageResult.ErrorMessage = "Invalid Extension";
                return imageResult;
            }

            try
            {
                //pass in whatever value you want
                if (isSacle)
                {
                    byte[] imgActual = Scale(file);

                    File.WriteAllBytes(path, imgActual);
                }
                else
                {
                    File.WriteAllBytes(path, file);
                }

                imageResult.ImageName = fileName;

                return imageResult;
            }
            catch (Exception ex)
            {
                imageResult.Success = false;
                imageResult.ErrorMessage = ex.Message;
                return imageResult;
            }
        }

        private BitmapFrame FastResize(BitmapFrame bfPhoto, float nWidth, float nHeight)
        {
            TransformedBitmap tbBitmap = new TransformedBitmap(bfPhoto, new ScaleTransform(nWidth / bfPhoto.Width, nHeight / bfPhoto.Height, 0, 0));
            return BitmapFrame.Create(tbBitmap);
        }

        private byte[] ToByteArray(BitmapFrame bfResize)
        {

            MemoryStream msStream = new MemoryStream();
            PngBitmapEncoder pbdDecoder = new PngBitmapEncoder();
            pbdDecoder.Frames.Add(bfResize);
            pbdDecoder.Save(msStream);
            return msStream.ToArray();
        }

        private BitmapFrame ReadBitmapFrame(Stream streamPhoto)
        {
            BitmapDecoder bdDecoder = BitmapDecoder.Create(streamPhoto, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
            return bdDecoder.Frames[0];
        }

        private bool ValidateExtension(string extension)
        {
            extension = extension.ToLower();
            switch (extension)
            {
                case ".jpg":
                    return true;
                case ".png":
                    return true;
                case ".gif":
                    return true;
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }

        private byte[] Scale(byte[] bytes)
        {
            Image imgPhoto;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                imgPhoto = Image.FromStream(ms);

            }

            float sourceWidth = imgPhoto.Width;
            float sourceHeight = imgPhoto.Height;
            float destHeight = 0;
            float destWidth = 0;

            // force resize, might distort image
            if (Width != 0 && Height != 0)
            {
                destWidth = Width;
                destHeight = Height;
            }
            // change size proportially depending on width or height
            else if (Height != 0)
            {
                destWidth = (float)(Height * sourceWidth) / sourceHeight;
                destHeight = Height;
            }
            else
            {
                destWidth = Width;
                destHeight = (float)(sourceHeight * Width / sourceWidth);
            }
            Stream streamPhoto = new MemoryStream(bytes);
            BitmapFrame bfPhoto = ReadBitmapFrame(streamPhoto);
            BitmapFrame bfResize = FastResize(bfPhoto, destWidth, destHeight);
            byte[] baResize = ToByteArray(bfResize);

            return baResize;
        }

    }
    public class ImageResult
    {
        public bool Success { get; set; }
        public string ImageName { get; set; }
        public string ErrorMessage { get; set; }
    }
}