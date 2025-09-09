using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform destination;

    public Transform GetDestination()
    {
        return destination;  // Allows you to go from one to place to the next (Telporter 1 -> Teleporter 2)
    }
}
