using Neuro.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Neuro.Extensions
{
    public static class MnistReader
    {
        private const string TrainImages = "Learning/t10k-images.idx3-ubyte";
        private const string TrainLabels = "Learning/t10k-labels.idx1-ubyte";

        public static IEnumerable<MnistImage> Read(string imagesPath = TrainImages, string labelsPath = TrainLabels)
        {
            BinaryReader labels = new BinaryReader(new FileStream(labelsPath, FileMode.Open));
            BinaryReader images = new BinaryReader(new FileStream(imagesPath, FileMode.Open));

            int magicNumber = images.ReadBigInt32();
            int numberOfImages = images.ReadBigInt32();
            int width = images.ReadBigInt32();
            int height = images.ReadBigInt32();

            int magicLabel = labels.ReadBigInt32();
            int numberOfLabels = labels.ReadBigInt32();

            var result = new List<MnistImage>();

            for (int i = 0; i < numberOfImages; i++)
            {
                var bytes = images.ReadBytes(width * height);
                var arr = new byte[height, width];

                arr.ForEach((j, k) => arr[j, k] = bytes[j * height + k]);

                result.Add(new MnistImage()
                {
                    Data = arr,
                    Label = labels.ReadByte()
                });
            }

            return result;
        }

        private static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }

        private static void ForEach<T>(this T[,] source, Action<int, int> action)
        {
            var width = source.GetLength(0);
            var height = source.GetLength(1);

            for (int w = 0; w < width; w++)
            {
                for (int h = 0; h < height; h++)
                {
                    action(w, h);
                }
            }
        }
    }
}
