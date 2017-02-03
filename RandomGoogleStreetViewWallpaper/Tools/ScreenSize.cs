namespace TG.Tools
{
    using System.Windows.Forms;

    public class ScreenSize
    {
        public ScreenSize(int x, int y)
        {
            this.SizeX = x;
            this.SizeY = y;
        }

        public int SizeX { get; }

        public int SizeY { get; }

        public static ScreenSize GetMainDisplayScreenSize()
        {
            return new ScreenSize(
                Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
        }
    }
}
