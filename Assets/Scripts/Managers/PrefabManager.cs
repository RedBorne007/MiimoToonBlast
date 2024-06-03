using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : Singleton<PrefabManager>
{
    [field: SerializeField] public NormalTile NormalTile { get; private set; }
    [field: SerializeField] public BombTile BombTile { get; private set; }
    [field: SerializeField] public DiscoTile DiscoTile { get; private set; }

    [field: Space]

    [field: SerializeField] public ParticlePool BreakParticle { get; private set; }
}
