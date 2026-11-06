using UnityEngine;

public class Plateform : MonoBehaviour
{
    public float speed = 5f;
    public Vector3 direction = Vector3.forward;

    void Update()
    {
        if (speed !=0)
            transform.Translate(-direction * speed * Time.deltaTime);
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    public void InvertDirection()
    {
        direction = Vector3.right;
    }
}
