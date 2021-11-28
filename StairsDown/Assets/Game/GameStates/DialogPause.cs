using GameGUI;

public class DialogPause : GUIScreenBase
{
    public void OnBtnResume()
    {
        SimpleGui.PopScreen(name);
        StateGameplay.Instance.OnResume();
    }

    public void OnBtnExit()
    {
        SimpleGui.PopScreen(name);
        StateGameplay.Instance.ExitToMenu();
    }
}
