namespace TG.Maps
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Threading;
    using TG.Maps.JsonApi;
    using TG.Tools;

    public class StreetViewLocation : 
        ICloneable,
        IComparable,
        IComparable<StreetViewLocation>,
        IEquatable<StreetViewLocation>
    {
        private const string ApiUri = "https://maps.googleapis.com/maps/api/streetview";

        private const string MetadataApiUri = "https://maps.googleapis.com/maps/api/streetview/metadata";

        private MapsMetadata metadata = null;

        public StreetViewLocation(
            ScreenSize size,
            double longitude,
            double latitude,
            double heading,
            double pitch,
            double scale)
        {
            this.IsLatitudeLongitude = true;
            this.ScreenSize = size;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Pitch = pitch;
            this.Heading = heading;
            this.Scale = scale;
        }

        public StreetViewLocation(
            ScreenSize size,
            string location,
            double heading,
            double pitch,
            double scale)
        {
            this.IsLatitudeLongitude = false;
            this.ScreenSize = size;
            this.Location = location;
            var metadata = this.Metadata;
            Debug.WriteLine($"Downloaded metadata: {metadata.PanoramaId}.");
            this.Pitch = pitch;
            this.Heading = heading;
            this.Scale = scale;
        }
        
        public static readonly string GlobalCachingLocation = Path.Combine(Directory.GetCurrentDirectory(), "gmaps");

        public ScreenSize ScreenSize { get; private set; }

        public bool IsLatitudeLongitude { get; private set; } = true;

        public double Longitude { get; private set; } = 46.414382;

        public double Latitude { get; private set; } = 10.013988;

        public string Location { get; private set; } = "Uffikon";

        public double Heading { get; private set; } = 0.0;

        public double Pitch { get; private set; } = 0.0;

        public double Scale { get; private set; } = 1.0;

        public int FieldOfView { get; private set; } = 120;

        public MapsMetadata Metadata
        {
            get
            {
                if (this.metadata == null)
                {
                    this.metadata = this.GetMetadata();
                    if (!this.IsLatitudeLongitude)
                    {
                        try
                        {
                            this.Latitude = this.metadata.Location.Latitude;
                            this.Longitude = this.metadata.Location.Longitude;
                        }
                        catch
                        {
                        }
                    }

                    if (this.metadata.Status == "OVER_QUERY_LIMIT")
                    {
                        Environment.Exit(-1);
                    }
                }

                return this.metadata;
            }
        }

        public bool DoesExist() =>
            this.Metadata.Status == "OK";

        public string GetLocalImageDirectory()
        {
            var imageDirectoryPath = Path.Combine(StreetViewLocation.GlobalCachingLocation, $"lat{this.Metadata.Location.Latitude}-lng{this.Metadata.Location.Longitude}");
            if (!Directory.Exists(imageDirectoryPath))
            {
                Directory.CreateDirectory(imageDirectoryPath);
            }

            return imageDirectoryPath;
        }

        public string DownloadStreetViewLocationImage()
        {
            var imagePath = Path.Combine(this.GetLocalImageDirectory(), $"lat{this.Metadata.Location.Latitude}-lng{this.Metadata.Location.Longitude}-ptc{this.Pitch}-hdg{this.Heading}-scl{this.Scale}-szx{this.ScreenSize.SizeX}-szy{this.ScreenSize.SizeY}.jpg");
            if (!File.Exists(imagePath))
            {
                var done = false;
                do
                {
                    try
                    {
                        new WebClient().DownloadFile(this.BuildUrl(), imagePath);
                        done = true;
                    }
                    catch
                    {
                        done = false;
                    }
                }
                while (!done);
            }

            return imagePath;
        }

        public string BuildUrl() =>
            $"{StreetViewLocation.ApiUri}{this.BuildUrlParameters()}";

        public object Clone() =>
            this.MemberwiseClone();

        public StreetViewLocation CloneLocation() =>
            (StreetViewLocation)this.Clone();

        public StreetViewLocation CloneLocationWithOverwrite(double pitch, double scale, double heading, double? lat = null, double? lgn = null)
        {
            var cloned = this.CloneLocation();
            cloned.Pitch = pitch;
            cloned.Scale = scale;
            cloned.Heading = heading;
            cloned.Latitude = lat.GetValueOrDefault(this.Latitude);
            cloned.Longitude = lgn.GetValueOrDefault(this.Longitude);
            return cloned;
        }

        public override bool Equals(object obj) =>
            (obj is StreetViewLocation location) ? this.Equals(location) : false;

        public override int GetHashCode() =>
            this.IsLatitudeLongitude ? this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode() : this.Location.GetHashCode();

        public bool Equals(StreetViewLocation other) =>
            other.IsLatitudeLongitude != this.IsLatitudeLongitude ?
                false : 
                (other.IsLatitudeLongitude ?
                    this.Latitude == other.Latitude && this.Longitude == other.Longitude :
                    this.Location == other.Location);

        public MapsMetadata GetMetadata() =>
            JsonConvert.DeserializeObject<MapsMetadata>(
                new WebClient().DownloadString($"{StreetViewLocation.MetadataApiUri}{this.BuildUrlParameters()}"), 
                new JsonSerializerSettings { DateFormatString = "YYYY-MM" });

        public int CompareTo(object obj) =>
            (obj is StreetViewLocation location) ?
                (location.IsLatitudeLongitude ?
                    location.Latitude.CompareTo(this.Latitude) :
                    location.Location.CompareTo(this.Location)) : -1;

        public int CompareTo(StreetViewLocation other) =>
            this.CompareTo(other);

        private string BuildUrlParameters() =>
            $"?size={this.ScreenSize.SizeX}x{this.ScreenSize.SizeY}&location={(this.IsLatitudeLongitude ? $"{this.Longitude},{this.Latitude}" : this.Location)}&heading={this.Heading}&pitch={this.Pitch}&fov={this.FieldOfView}&scale={this.Scale}&key={Properties.Settings.Default.ApiKey}";
    }
}