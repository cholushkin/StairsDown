using GameLib.Random;
using UnityEngine;

namespace TowerGenerator
{
    public class ConfigLineUp : GeneratorConfigBase
    {
        public Range IslandHeight;
        public Range IslandDistanceFromTrunk;

        [Range(0f, 1f)]
        public float PropagateIslandChance; // used for creating openings on branches(use case 1) and for continue openings from prev generator (2)

        public override GeneratorBase GetGenerator()
        {
            if (_generator == null)
                _generator = new GeneratorLineUp(this);
            return _generator;
        }

        private GeneratorBase _generator;
    }

}