using UnityEngine;

public class StateGameplay : AppStateManager.AppState<StateGameplay>
{

    // todo: better implenentation
    public GameObject GamePrefab;

    private GameObject _game;

    public override void AppStateEnter()
    {
        gameObject.SetActive(true);
        _game = Instantiate(GamePrefab, transform);
        //CameraGameplay.Instance.Focus(StairsGeneratorProcessor.Instance.CurrentChunk.transform);
    }

    public override void AppStateLeave()
    {
        Destroy(_game);
        _game = null;
        gameObject.SetActive(false);
    }
}
