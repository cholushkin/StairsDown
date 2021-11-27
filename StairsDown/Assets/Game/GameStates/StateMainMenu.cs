public class StateMainMenu : AppStateManager.AppState<StateMainMenu>
{
    public void StartGame()
    {
        AppStateManager.Instance.Start<StateGameplay>();
    }

    public override void AppStateEnter()
    {
        gameObject.SetActive(true);
    }

    public override void AppStateLeave()
    {
        gameObject.SetActive(false);
    }
}
