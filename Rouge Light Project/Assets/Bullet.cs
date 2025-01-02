using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float timeDestroy = 3f;
    public float speed = 3f;
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Vector3 diference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotateZ = Mathf.Atan2(diference.y, diference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);

        rb.linearVelocity = transform.up * speed;
        Invoke("DestroyBullet", timeDestroy);
    }

    void DestroyBullet()
    {
        Destroy(this.gameObject);
    }
}