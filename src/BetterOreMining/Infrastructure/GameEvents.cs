using System;
using BetterOreMining.Features;
using UnityEngine;

namespace BetterOreMining.Infrastructure
{
    public sealed class GameEvents : IGameEvents
    {
        internal static GameEvents Instance { get; set; }

        public event Action<Attack> MeleeAttackStarting;
        public event Action<Vector3> PickaxeStruck;
        public event Action FrameUpdated;

        internal void RaiseMeleeAttackStarting(Attack attack)
        {
            MeleeAttackStarting?.Invoke(attack);
        }

        internal void RaisePickaxeStruck(Vector3 point)
        {
            PickaxeStruck?.Invoke(point);
        }

        internal void RaiseFrameUpdated()
        {
            FrameUpdated?.Invoke();
        }
    }
}
