namespace Neuro.Extensions
{
    public static class DoubleExtension
    {
        public static T[] ToLinearArray<T>(this T[][,] outputs) where T : struct
        {
            var imageHeight = outputs[0].GetLength(0);
            var imageWidth = outputs[0].GetLength(1);
            var result = new T[outputs.Length * imageHeight * imageWidth];

            for (var i = 0; i < outputs.Length; i++)
            {
                for (var h = 0; h < imageHeight; h++)
                {
                    for (var w = 0; w < imageWidth; w++)
                    {
                        var position = (i * (imageWidth * imageHeight)) + (h * imageWidth + w);
                        result[position] = outputs[i][h, w];
                    }
                }
            }

            return result;
        }
    }
}
