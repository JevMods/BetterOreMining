using System.Collections.Generic;

namespace BetterOreMining.Features
{
    public interface IGlowRenderer
    {
        void Draw(IReadOnlyList<RippleWave> waves, float ringWidth);

        void Clear();
    }
}
