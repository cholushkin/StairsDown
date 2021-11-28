using GameGUI;
using UnityEngine;

public class OverlayInGame : GUIScreenBase
{

    public void OnBtnPause()
    {
        StateGameplay.Instance.OnPause();
    }

    void Update()
    {
        if (IsInputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBtnPause();
            }
        }
    }
}
