namespace TG.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TG.Maps;

    public class StreetViewImageApplierService : IService
    {
        public void Start()
        {
            if (!Directory.Exists(StreetViewLocation.GlobalCachingLocation))
            {
                Directory.CreateDirectory(StreetViewLocation.GlobalCachingLocation);
            }

            Task.Run(() => this.CheckAndApplyLocalFileSystemStreetViewImages());
        }

        private void CheckAndApplyLocalFileSystemStreetViewImages()
        {
            while (true)
            {
                var directories = Directory.GetDirectories(StreetViewLocation.GlobalCachingLocation, "*", SearchOption.TopDirectoryOnly).ToList();
                if (directories.Count > 0)
                {
                    var directory = directories.OrderBy(x => Directory.GetFiles(x).Count() == 360).ThenBy(x => Guid.NewGuid()).First();
                    var path = Path.Combine(StreetViewLocation.GlobalCachingLocation, directory);
                    var files = Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly);
                    var ordered = files.OrderBy(x => x).ToList();
                    if (ordered.Count == 359)
                    {
                        foreach (var item in ordered)
                        {
                            WallpaperChangingProvider.SetWallpaper(item);
                            Thread.Sleep(50);
                        }
                    }
                }
            }
        }
    }
}