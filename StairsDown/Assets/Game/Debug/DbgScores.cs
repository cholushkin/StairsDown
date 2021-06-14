
public class DbgScores : Pane
{
    private int _prevScore = -1;

    public override void Update()
    {
        if(StairsGeneratorProcessor.Instance == null)
            return;
        if (StairsGeneratorProcessor.Instance.SpawnedChunksCount != _prevScore)
        {
            SetText($"Scores {StairsGeneratorProcessor.Instance.SpawnedChunksCount}");
            _prevScore = StairsGeneratorProcessor.Instance.SpawnedChunksCount;
        }
    }
}
