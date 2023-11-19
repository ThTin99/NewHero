using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float rotateSpeed = 150f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float radiusSpeed = 1000f;
    [SerializeField] private bool clockWise;

    private Vector3 desiredPosition;

    private void Start()
    {
        center = GameObject.FindGameObjectWithTag("Player").transform;

        transform.position = (center.position - transform.position).normalized * radius + center.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //move with center
        desiredPosition = (transform.position - center.position).normalized * radius + center.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, radiusSpeed * Time.deltaTime);

        //rotate around center
        if (clockWise)
        {
            transform.RotateAround(center.position, Vector3.up, rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.RotateAround(center.position, Vector3.up, -rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ISlowable slowable))
        {
            slowable.ApplySlowEffect(20, 3f);
        }
    }
}
