using UnityEngine;
using UnityEngine.Events;

public class LeverSystem : MonoBehaviour
{
    [Header("Settings")]
    public bool isOn = false;

    [Header("Events")]
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;
    
    public void Toggle()
    {
        isOn = !isOn;
        if (isOn) onActivate.Invoke();
        else onDeactivate.Invoke();
    }
    
    public void SetOn()
    {
        isOn = true;
        onActivate.Invoke();
    }
    
    public void SetOff()
    {
        isOn = false;
        onDeactivate.Invoke();
    }
}
