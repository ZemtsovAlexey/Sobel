namespace Sobel.Models
{
    public class PixelBright
    {
        public PixelBright()
        {
        }

        public PixelBright(int x, int y, int bright)
        {
            X = x;
            Y = y;
            Bright = bright;
        }
        
        public int X { get; set; }
        
        public int Y { get; set; }
        
        public int Bright { get; set; }
    }
}