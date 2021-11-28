using UnityEngine;
using UnityEngine.EventSystems;

public class DefaultFocus : MonoBehaviour
{
    void OnEnable() => EventSystem.current.SetSelectedGameObject(gameObject, null);

}
