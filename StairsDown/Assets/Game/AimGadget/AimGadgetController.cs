using System.Collections;
using System.Collections.Generic;
using Alg;
using UnityEngine;

public class AimGadgetController : Singleton<AimGadgetController>
{
    public AimGadgetView View;
    public bool StatusActive { get; private set; }

    public void Attach(Transform attachPos)
    {
        StatusActive = true;
        transform.position = attachPos.position + Vector3.left * 1.0f;
        View.PlayAppearAnimation();
        DoRandomActivity();
    }

    private void DoRandomActivity()
    {
        int option = Random.Range(1, 5);
        switch (option)
        {
            case 1:
                View.ConnectorElevate();
                break;
            case 2:
                View.ConnectorSideMove();
                break;
            case 3:
                View.ConnectorMoveForward();
                break;
            case 4:
                View.ConnectorRoll();
                break;
        }
    }

    public void Hide()
    {
        StatusActive = false;
        View.PlayDisappearAnimation();
    }

    public Vector3 GetOffset()
    {
        return View.Connector.position - transform.position;
    }

    public float GetRotation()
    {
        return View.Connector.rotation.eulerAngles.z - 90;
    }
}
