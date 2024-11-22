using UnityEngine;

namespace Local.Noah.Scripts
{
    public static class Noise
    {
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float lacunarity)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];

            System.Random random = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = random.Next(-100000, 100000);
                float offsetY = random.Next(-100000, 100000);
                octaveOffsets[i] = new Vector2(offsetX, offsetY);
            }

            if (scale <= 0)
            {
                scale = 0.0001f;
            }

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    float noiseHeight = 0;
                    float amplitude = 1;
                    float frequency = 1;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x + octaveOffsets[i].x) / scale * frequency;
                        float sampleY = (y + octaveOffsets[i].y) / scale * frequency;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        frequency *= lacunarity;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }

            return noiseMap;
        }
    }
}