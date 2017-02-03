namespace TG.Services
{
    using System.Threading.Tasks;
    using TG.Tools;

    public class WallpaperChangingService : IService
    {
        public void Start()
        {
            Task.Run(() => new StreetViewDownloaderService(ScreenSize.GetMainDisplayScreenSize()).Start());
            Task.Run(() => new StreetViewImageApplierService().Start());
        }
    }
}
