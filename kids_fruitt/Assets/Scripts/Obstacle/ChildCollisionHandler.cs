using UnityEngine;

public class ChildCollisionHandler : MonoBehaviour
{
    private RotatingObstacle parentObstacle;

    private void Start()
    {
        parentObstacle = GetComponentInParent<RotatingObstacle>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        parentObstacle?.HandleCollision(collision);
    }
}
