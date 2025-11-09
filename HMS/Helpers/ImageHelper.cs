using System;
using System.Drawing;
using System.IO;

namespace HMS.Helpers
{
    /// <summary>
    /// Utility class for image conversion operations
    /// Provides static methods for converting between Image and byte arrays
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Converts an Image to a byte array (PNG format)
        /// </summary>
        /// <param name="image">The image to convert</param>
        /// <returns>Byte array representing the image, or null if image is null</returns>
        public static byte[] ImageToByteArray(Image image)
        {
            if (image == null) 
                return null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Converts a byte array to an Image
        /// </summary>
        /// <param name="bytes">The byte array to convert</param>
        /// <returns>Image object, or null if bytes is null or empty</returns>
        public static Image ByteArrayToImage(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) 
                return null;

            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    return Image.FromStream(ms);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

