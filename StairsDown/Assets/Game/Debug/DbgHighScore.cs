
public class DbgHighScore : Pane
{
    public bool SaveHighScore; // todo: implement me
    private static int _highScores;

    public override void InitializeState()
    {
        base.InitializeState();
        SetText($"Highscore {_highScores}");
    }

    public override void Update()
    {
        if (StairsGeneratorProcessor.Instance == null)
            return;
        if (StairsGeneratorProcessor.Instance.SpawnedChunksCount > _highScores)
        {
            _highScores = StairsGeneratorProcessor.Instance.SpawnedChunksCount;
            SetText($"Highscore {_highScores}");
        }
    }
}
