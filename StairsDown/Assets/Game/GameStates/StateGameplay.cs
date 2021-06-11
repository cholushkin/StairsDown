using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateGameplay : AppStateManager.AppState<StateGameplay>
{
    public override void AppStateEnter()
    {
        //CameraGameplay.Instance.Focus(StairsGeneratorProcessor.Instance.CurrentChunk.transform);
    }

    public override void AppStateLeave()
    {

    }
}
