using UnityEngine;

namespace BetterOreMining.Features
{
    public sealed class GroundMap
    {
        public const int Resolution = 32;

        private GroundMap(Vector2 origin, float size, float[] heights)
        {
            Origin = origin;
            Size = size;
            Heights = heights;
        }

        public Vector2 Origin { get; }

        public float Size { get; }

        public float[] Heights { get; }

        public static GroundMap Sample(IGroundProbe ground, Vector3 center, float radius)
        {
            var size = radius * 2f;
            var cell = size / Resolution;
            var origin = new Vector2(center.x - radius, center.z - radius);
            var heights = new float[Resolution * Resolution];
            for (var row = 0; row < Resolution; row++)
            {
                for (var column = 0; column < Resolution; column++)
                {
                    var point = new Vector3(
                        origin.x + (column + 0.5f) * cell,
                        center.y,
                        origin.y + (row + 0.5f) * cell);
                    heights[row * Resolution + column] = ground.GroundHeight(point);
                }
            }

            return new GroundMap(origin, size, heights);
        }
    }
}
