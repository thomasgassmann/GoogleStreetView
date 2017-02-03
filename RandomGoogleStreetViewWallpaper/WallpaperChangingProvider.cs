namespace TG
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;

    public static class WallpaperChangingProvider
    {
        private const int SPI_SETDESKWALLPAPER = 20;

        private const int SPIF_UPDATEINIFILE = 0x1;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uiAction, int uiParam, string pvParam, int fWinIni);
        
        public static void SetWallpaper(Uri uri)
        {
            using (var webClient = new WebClient())
            {
                var file = Path.GetTempFileName();
                webClient.DownloadFile(uri.ToString(), file);
                WallpaperChangingProvider.SetWallpaper(file);
            }
        }

        public static void SetWallpaper(Image image)
        {
            var path = Path.GetTempFileName();
            image.Save(path);
            WallpaperChangingProvider.SetWallpaper(path);
        }

        public static void SetWallpaper(string filePath)
        {
            WallpaperChangingProvider.SystemParametersInfo(
                WallpaperChangingProvider.SPI_SETDESKWALLPAPER,
                0,
                filePath,
                WallpaperChangingProvider.SPIF_UPDATEINIFILE);
        }
    }
}
