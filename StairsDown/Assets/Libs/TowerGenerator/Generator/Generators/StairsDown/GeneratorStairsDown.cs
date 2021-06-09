using GameLib.Random;
using TowerGenerator;
using UnityEngine;
using UnityEngine.Assertions;

public class GeneratorStairsDown : GeneratorBase
{
    public GeneratorStairsDown(GeneratorConfigBase cfg) : base(cfg)
    {
    }

    public override void Generate(GeneratorProcessor.State state, int iteration)
    {
        Assert.IsTrue(state.OpenedSegments.Count == 1);
        var openedNode = state.OpenedSegments[0];

        Assert.IsTrue(openedNode.BranchLevel == 0);
        var architect = new SegmentArchitect(
            _rndTopology.ValueInt(), _rndVisual.ValueInt(), _rndContent.ValueInt(),
            state.Pointers.PointerGeneratorStable, Config,
            TopologyType.ChunkStd, TopologyType.ChunkStd, TopologyType.ChunkStd
        );

        var success = architect.MakeProjects(openedNode, Config.TrunkSegmentsCount, Vector3.zero, Vector3.forward);
        Assert.IsTrue(success);
        var project = architect.GetProject(_rndTopology.Range(0, architect.GetProjectVariantsNumber()), out var lastSeg);

        state.Blueprint.AddSubtree(openedNode, Vector3.up, project);
        state.OpenedSegments.Remove(openedNode);
        state.OpenedSegments.Add(lastSeg);
    }

    public override bool Done()
    {
        return true;
    }
}
