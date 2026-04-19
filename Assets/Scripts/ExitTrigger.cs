using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public bool isCatInside = false;
    public string targetCatLayer; // "AliveCat" ou "DeadCat"

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == targetCatLayer)
            isCatInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == targetCatLayer)
            isCatInside = false;
    }
}