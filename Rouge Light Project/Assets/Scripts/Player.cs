using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 0.5f;
    private Vector2 moveVector;
    public GameObject Bullet;
    public Transform firePoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveVector.x = Input.GetAxis("Horizontal");
        moveVector.y = Input.GetAxis("Vertical");
        rb.MovePosition(rb.position + moveVector * speed * Time.deltaTime);

        

        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instantiate(Bullet, firePoint.position, firePoint.rotation);
        }*/

        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        // �������� ������� ������� � ������� �����������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // ��������� ����������� �� ������ � �������
        Vector2 direction = mousePosition - transform.position;

        // ��������� ���� ��� ��������
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        // ������������� ����� ���� ��������
        rb.rotation = angle;
    }
}