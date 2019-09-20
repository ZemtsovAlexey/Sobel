namespace Sobel.Models
{
    public class Symble
    {
        public Symble(float[,] img, string value)
        {
            Img = img;
            Value = value;
        }
	
        public float[,] Img { get; set; }
        
        public string Value {get;set;}	
    }
}