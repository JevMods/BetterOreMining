using UnityEngine;

namespace BetterOreMining.Features
{
    public interface IGroundProbe
    {
        float GroundHeight(Vector3 point);
    }
}
