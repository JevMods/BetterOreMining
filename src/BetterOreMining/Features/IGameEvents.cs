using System;
using UnityEngine;

namespace BetterOreMining.Features
{
    public interface IGameEvents
    {
        event Action<Attack> MeleeAttackStarting;
        event Action<Vector3> PickaxeStruck;
        event Action FrameUpdated;
    }
}
