using UnityEngine;

namespace BetterOreMining.Features
{
    public interface IVeinScanner
    {
        VeinScan Scan(Vector3 center, float radius);
    }
}
