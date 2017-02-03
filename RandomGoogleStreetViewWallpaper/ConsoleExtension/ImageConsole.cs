namespace TG.ConsoleExtension
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;

    public static class ImageConsole
    {
        private static int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

        public static void WriteImage(string url)
        {
            using (var webClient = new WebClient())
            {
                var path = Path.GetTempFileName();
                webClient.DownloadFile(url, path);
                ImageConsole.WriteImage((Bitmap)Image.FromFile(path));
            }
        }

        public static void WritePixel(Color cValue)
        {
            var cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
            var rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 };
            var bestHit = new int[] { 0, 0, 4, int.MaxValue };
            for (var rChar = rList.Length; rChar > 0; rChar--)
            {
                for (var cFore = 0; cFore < cTable.Length; cFore++)
                {
                    for (var cBack = 0; cBack < cTable.Length; cBack++)
                    {
                        var R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                        var G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                        var B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                        var iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                        if (!(rChar > 1 && rChar < 4 && iScore > 50000))
                        {
                            if (iScore < bestHit[3])
                            {
                                bestHit[3] = iScore;
                                bestHit[0] = cFore;
                                bestHit[1] = cBack;
                                bestHit[2] = rChar;
                            }
                        }
                    }
                }
            }

            Console.ForegroundColor = (ConsoleColor)bestHit[0];
            Console.BackgroundColor = (ConsoleColor)bestHit[1];
            Console.Write(rList[bestHit[2] - 1]);
        }


        public static void WriteImage(Bitmap source)
        {
            var sMax = 39;
            var percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
            var dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
            var bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
            for (var i = 0; i < dSize.Height; i++)
            {
                for (var j = 0; j < dSize.Width; j++)
                {
                    ImageConsole.WritePixel(bmpMax.GetPixel(j * 2, i));
                    ImageConsole.WritePixel(bmpMax.GetPixel(j * 2 + 1, i));
                }

                System.Console.WriteLine();
            }

            Console.ResetColor();
        }
    }
}
