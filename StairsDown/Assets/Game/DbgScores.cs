
public class DbgScores : Pane

{
    public override void Update()
    {
        if(StairsGeneratorProcessor.Instance == null)
            return;
        SetText($"Scores {StairsGeneratorProcessor.Instance.SpawnedChunksCount}");
    }
}
