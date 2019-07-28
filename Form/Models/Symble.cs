namespace Sobel.Models
{
    public class Symble
    {
        public Symble(double[,] img, string value)
        {
            Img = img;
            Value = value;
        }
	
        public double[,] Img { get; set; }
        
        public string Value {get;set;}	
    }
}