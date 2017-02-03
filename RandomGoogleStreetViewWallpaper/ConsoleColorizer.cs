namespace TG
{
    using System;

    public class ConsoleColorizer : IDisposable
    {
        public ConsoleColorizer(ConsoleColor color)
        {
            this.OldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
        }

        public ConsoleColor OldColor { get; }

        public void Dispose()
        {
            Console.ForegroundColor = this.OldColor;
        }
    }
}
