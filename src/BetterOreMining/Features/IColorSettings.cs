using UnityEngine;

namespace BetterOreMining.Features
{
    public interface IColorSettings
    {
        Color VisibleColor { get; }

        Color BlockedColor { get; }
    }
}
