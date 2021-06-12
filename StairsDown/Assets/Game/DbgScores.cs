
public class DbgScores : Pane

{
    public override void Update()
    {
        SetText($"Scores {StairsGeneratorProcessor.Instance.SpawnedChunksCount}");
    }
}
