namespace TG
{
    using ConsoleExtension;
    using System;
    using System.Globalization;
    using System.Linq;
    using TG.Services;

    public class Program
    {
        public static void Main()
        {
            using (new ConsoleColorizer(ConsoleColor.Yellow))
            {
                Colorful.Console.WriteAscii($"#{DateTime.Now.Year} - Thomas");
            }

            ImageConsole.WriteImage("https://pbs.twimg.com/profile_images/809064074998710272/KJvmreRz.jpg");
            using (new ConsoleColorizer(ConsoleColor.Green))
            {
                if (Properties.Settings.Default.ApiKey == string.Empty)
                {
                    Console.WriteLine("Please provide your API key for the Google Street View Image API. https://console.developers.google.com");
                    Properties.Settings.Default.ApiKey = Console.ReadLine();

                }
                new WallpaperChangingService().Start();
            }

            Console.ReadLine();
        }

        public static string GetResult(params string[] possibleSolutions)
        {
            var result = default(string);
            using (new ConsoleColorizer(ConsoleColor.Yellow))
            {
                while (!possibleSolutions.Contains(result, StringComparer.Create(CultureInfo.CurrentCulture, true)))
                {
                    Console.WriteLine("Please type in one of the following possible answers:");
                    possibleSolutions.ToList().ForEach(x => Console.Write($"{x}; "));
                    using (new ConsoleColorizer(ConsoleColor.White))
                    {
                        result = Console.ReadLine();
                    }
                }
            }

            return possibleSolutions.First(x => result.Equals(x, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
