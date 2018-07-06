namespace Neuro.Models
{
    public struct Сoordinate
    {
        public Сoordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        public int X { get; set; }
        public int Y { get; set; }
    }
}