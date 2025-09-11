using UnityEngine;

public class MovePlat : MonoBehaviour
{

    public Transform pointA;
    public Transform pointB;
    [Header("Speed")]
    [SerializeField] public float moveSpeed = 2f;

    private Vector3 nextPosition;

    void Start()
    {
        nextPosition = pointB.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, moveSpeed * Time.deltaTime);

        if(transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
            return;
        }
    }
}
