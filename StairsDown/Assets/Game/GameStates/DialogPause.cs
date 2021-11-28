using GameGUI;
using UnityEngine;

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

    void Update()
    {
        if (IsInputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBtnResume();
            }
        }
    }
}
