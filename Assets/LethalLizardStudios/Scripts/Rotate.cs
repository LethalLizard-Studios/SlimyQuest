using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private Vector3 direction = Vector3.down;
    [SerializeField] private int speed = 8;

    private void FixedUpdate()
    {
        transform.Rotate(direction, speed);
    }
}
