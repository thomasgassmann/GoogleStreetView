namespace TG.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using TG.Maps;
    using TG.Tools;

    public class StreetViewDownloaderService : IService
    {
        private readonly Random random = new Random();

        private readonly ConcurrentBag<StreetViewLocation> locationList = new ConcurrentBag<StreetViewLocation>();

        public StreetViewDownloaderService(ScreenSize screenSize)
        {
            this.ScreenSize = screenSize;
        }

        public ScreenSize ScreenSize { get; }

        public void Start()
        {
            Task.Run(() => this.FindNearPlaces());
            while (true)
            {
                var location = this.CreateExistingRandomStreetViewLocation();
                this.locationList.Add(location);
                Task.Run(() => this.DownloadData(location));
            }
        }

        private void FindNearPlaces()
        {
            while (true)
            {
                var localCopy = this.locationList.ToList();
                foreach (var localItem in localCopy)
                {
                    var adjusted = this.AdjustCoordinates(localItem);
                    if (adjusted.DoesExist())
                    {
                        this.locationList.Add(adjusted);
                        Task.Run(() => this.DownloadData(adjusted));
                    }
                }
            }
        }

        private StreetViewLocation AdjustCoordinates(StreetViewLocation location)
        {
            return location.CloneLocationWithOverwrite(
                location.Pitch,
                location.Scale,
                location.Heading,
                location.Latitude - (this.random.NextDouble() - 0.5) / 30,
                location.Longitude - (this.random.NextDouble() - 0.5) / 30);
        }

        private void DownloadData(StreetViewLocation location)
        {
            var cloned = location.CloneLocationWithOverwrite(0, 1, 0);
            for (var i = 0; i < 360; i += 1)
            {
                cloned.DownloadStreetViewLocationImage();
                cloned = cloned.CloneLocationWithOverwrite(cloned.Pitch, cloned.Scale, i);
            }
        }

        private StreetViewLocation CreateExistingRandomStreetViewLocation()
        {
            while (true)
            {
                var location = this.CreateRandomStreetViewLocation();
                if (location.DoesExist())
                {
                    return location;
                }
            }
        }
        
        private StreetViewLocation CreateRandomStreetViewLocation()
        {
            if (this.random.NextDouble() < 0.75)
            {
                return new StreetViewLocation(
                    this.ScreenSize,
                    this.GetRandomDouble(0, 360) - 180,
                    this.GetRandomDouble(0, 180) - 90,
                    this.GetRandomDouble(0, 360),
                    this.GetRandomDouble(0, 40) - 20,
                    1);
            } 
            else
            {
                return new StreetViewLocation(
                    this.ScreenSize,
                    new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ", this.random.Next(2, 5))
                        .Select(s => s[random.Next(s.Length)]).ToArray()),
                    this.GetRandomDouble(0, 360),
                    this.GetRandomDouble(0, 40) - 20,
                    1);
            }
        }
        
        private double GetRandomDouble(double minimum, double maximum) =>
            this.random.NextDouble() * (maximum - minimum) + minimum;
    }
}