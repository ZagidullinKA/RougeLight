using UnityEngine;
//using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 0.5f;
    private Vector2 moveVector;
    public GameObject Bullet;
    public Transform firePoint;
    public Shooting shooting;

    private bool isShooting = false;
    //private InputAction shootAction;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        shooting = GetComponent<Shooting>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            isShooting = true;
        }
    }

    private void FixedUpdate()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");

        rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);
        RotateTowardsMouse();

        if (isShooting)
        {
            shooting.Shot(1, 0.5f);
            isShooting = false;
        }
    }

    void RotateTowardsMouse()
    {
        // ѕолучаем позицию курсора в мировых координатах
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ¬ычисл€ем направление от игрока к курсору
        Vector2 direction = mousePosition - transform.position;

        // ¬ычисл€ем угол дл€ поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // ”станавливаем новый угол поворота
        rb.rotation = angle;
    }
}