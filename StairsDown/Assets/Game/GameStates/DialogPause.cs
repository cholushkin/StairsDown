using GameGUI;

public class DialogPause : GUIScreenBase
{
    public void OnBtnResume()
    {
        SimpleGui.PopScreen(name);
        StateGameplay.Instance.OnResume();
    }
}
