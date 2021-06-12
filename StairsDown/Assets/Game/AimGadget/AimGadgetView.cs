using DG.Tweening;
using UnityEngine;



// todo: scale

public class AimGadgetView : MonoBehaviour
{
    public Transform Axis;
    public Transform VerticalAlongPath;
    public Transform VerticalPerpendicularPath;
    public Transform Horizontal;
    public Transform Connector;
    public float Aspect;

    [Header("Axis move")]
    public float AxisMoveDuration;
    public float AxisMoveDistance;
    public Ease AxisEase;

    [Header("Connector forward move")]
    public float ConnecortMoveForwardDuration;
    public Ease ConnectorMoveForwardEase;

    [Header("Connector up move")]
    public float ConnecortMoveUpDuration;
    public Ease ConnectorMoveUpEase;

    [Header("Connector side move")]
    public float ConnecortSideMoveDuration;
    public Ease ConnectorSideMoveEase;

    [Header("Roll connector")]
    public float ConnectorRollDuration;
    public float ConnectorRollAmplitude;
    public Ease ConnectorRollEase;


    public void PlayAppearAnimation()
    {
        Reset();
        float duration = 0.5f;
        transform.DOScale(Vector3.one, duration);
        //VerticalAlongPath.DOScale(0f, duration).From().SetEase(Ease.OutCubic);
        //VerticalPerpendicularPath.DOScale(0f, duration).From().SetEase(Ease.OutCubic);
        //Horizontal.DOScale(0f, duration).From().SetEase(Ease.OutCubic);
    }

    public void PlayDisappearAnimation()
    {
        Reset();
        float duration = 0.2f;
        transform.DOScale(Vector3.zero, duration);
        //VerticalAlongPath.DOScale(0f, duration).SetEase(Ease.OutCubic);
        //VerticalPerpendicularPath.DOScale(0f, duration).SetEase(Ease.OutCubic);
        //Horizontal.DOScale(0f, duration).SetEase(Ease.OutCubic);
    }

    [ContextMenu("MoveAxisAlongPath")]
    public void MoveAxisAlongPath()
    {
        Axis.DOLocalMove(Vector3.forward * AxisMoveDistance, AxisMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(AxisEase);
    }

    [ContextMenu("ConnectorMoveForward")]
    public void ConnectorMoveForward()
    {
        Connector.DOKill();
        Connector.localPosition = Vector3.zero;
        Connector.DOLocalMove(Vector3.forward * Aspect * 0.5f, ConnecortMoveForwardDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ConnectorMoveForwardEase)
            .Goto(ConnecortMoveForwardDuration * Random.value, true);
    }

    [ContextMenu("ConnectorRoll")]
    public void ConnectorRoll()
    {
        Connector.DOKill();
        Connector.localPosition = Vector3.zero;
        Connector.localRotation = Quaternion.Euler(0, 0, 90 - ConnectorRollAmplitude * 0.5f);
        Connector.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 90 + ConnectorRollAmplitude * 0.5f), ConnectorRollDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ConnectorRollEase)
            .Goto(ConnectorRollDuration * Random.value, true);
    }

    [ContextMenu("ConnectorElevate")]
    public void ConnectorElevate()
    {
        Connector.DOKill();
        Connector.localPosition = -Vector3.up * Aspect * 0.5f;
        Connector.DOLocalMove(Vector3.up * Aspect * 0.5f, ConnecortMoveUpDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ConnectorMoveUpEase)
            .Goto(ConnecortMoveUpDuration*Random.value,true);
    }

    [ContextMenu("ConnectorSideMove")]
    public void ConnectorSideMove()
    {
        Connector.DOKill();
        Connector.localPosition = -Vector3.left * Aspect * 0.5f;
        Connector.DOLocalMove(Vector3.left * Aspect * 0.5f, ConnecortSideMoveDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(ConnectorSideMoveEase)
            .Goto(ConnecortSideMoveDuration * Random.value, true);
    }



    private void Reset()
    {
        VerticalAlongPath.transform.localScale = Vector3.one;
        VerticalPerpendicularPath.transform.localScale = Vector3.one;
        Horizontal.transform.localScale = Vector3.one;
        Connector.localRotation = Quaternion.Euler(0, 0, 90);
    }


}
