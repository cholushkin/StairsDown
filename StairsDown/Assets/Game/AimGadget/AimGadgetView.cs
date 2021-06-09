using DG.Tweening;
using UnityEngine;

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
        float duration = 0.2f;
        VerticalAlongPath.DOScale(0f, duration).From().SetEase(Ease.OutCubic);
        VerticalPerpendicularPath.DOScale(0f, duration).From().SetEase(Ease.OutCubic);
        Horizontal.DOScale(0f, duration).From().SetEase(Ease.OutCubic);
    }

    public void PlayDisappearAnimation()
    {
        Reset();
        float duration = 0.2f;
        VerticalAlongPath.DOScale(0f, duration).SetEase(Ease.OutCubic);
        VerticalPerpendicularPath.DOScale(0f, duration).SetEase(Ease.OutCubic);
        Horizontal.DOScale(0f, duration).SetEase(Ease.OutCubic);
    }

    [ContextMenu("MoveAxisAlongPath")]
    public void MoveAxisAlongPath()
    {
        Axis.DOLocalMove(Vector3.forward * AxisMoveDistance, AxisMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(AxisEase);
    }

    [ContextMenu("ConnectorMoveForward")]
    public void ConnectorMoveForward()
    {
        Connector.DOLocalMove(Vector3.forward * Aspect * 0.5f, ConnecortMoveForwardDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ConnectorMoveForwardEase);
    }

    [ContextMenu("ConnectorRoll")]
    public void ConnectorRoll()
    {
        Connector.localRotation = Quaternion.Euler(0, 0, 90 - ConnectorRollAmplitude * 0.5f);
        Connector.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 90 + ConnectorRollAmplitude * 0.5f), ConnectorRollDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ConnectorRollEase);
    }

    [ContextMenu("ConnectorElevate")]
    public void ConnectorElevate()
    {
        Connector.localPosition = -Vector3.up * Aspect * 0.5f;
        Connector.DOLocalMove(Vector3.up * Aspect * 0.5f, ConnecortMoveUpDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ConnectorMoveUpEase);
    }

    [ContextMenu("ConnectorSideMove")]
    public void ConnectorSideMove()
    {
        Connector.localPosition = -Vector3.left * Aspect * 0.5f;
        Connector.DOLocalMove(Vector3.left * Aspect * 0.5f, ConnecortSideMoveDuration).SetLoops(-1, LoopType.Yoyo).SetEase(ConnectorSideMoveEase);
    }



    private void Reset()
    {
        VerticalAlongPath.transform.localScale = Vector3.one;
        VerticalPerpendicularPath.transform.localScale = Vector3.one;
        Horizontal.transform.localScale = Vector3.one;
    }


}
