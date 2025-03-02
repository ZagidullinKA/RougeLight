using log4net;
using Mono.Cecil.Cil;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyTestAI : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(EnemyTestAI));

    private Transform playerTransform; // Ссылка на героя
    private float minRadius = 3f; // Минимальный радиус (50 пикселей)
    private float maxRadius = 5f; // Максимальный радиус (100 пикселей)
    private float moveSpeed; // Скорость движения врага
    private float timer = 0f; // Направление движения
    private int directionCircle = 1; // Направление движения (1 — по часовой стрелке, -1 — против)
    private bool directionFront = true; // Направление движения (true — к герою, false — от героя)
    float changeInterval; // Текущий интервал смены направления
    Vector2 direction;

    private Rigidbody2D rb;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        Mobs mobsScript = gameObject.GetComponent<Mobs>();
        moveSpeed = mobsScript.MoveSpeed;
        changeInterval = Random.Range(1, 5);
        log.Debug("Инициализация скорости передвижения - " + moveSpeed + " mobsScript.MoveSpeed - " + mobsScript.MoveSpeed);
        rb = GetComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // Отключаем гравитацию
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Включаем непрерывное обнаружение коллизий
        rb.freezeRotation = true; // Запрещаем вращение
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            log.Debug("Столкновение с: " + collision.gameObject.name + " Tag - " + collision.gameObject.tag);
            if (collision.gameObject.CompareTag("Enemy"))
            {
                directionCircle *= -1;
                // Рассчитываем направление отталкивания
                Vector2 pushDirection = (rb.position - (Vector2)collision.transform.position).normalized;

                // Применяем отталкивание через AddForce
                rb.AddForce(pushDirection * moveSpeed * 50f, ForceMode2D.Impulse); // Используем импульс для отталкивания
            }
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Герой не назначен!");
            return;
        }

        // Расстояние до героя
        float distanceToHero = Vector2.Distance(transform.position, playerTransform.position);
        direction = (transform.position - playerTransform.position).normalized;

        // Логика движения
        if (distanceToHero > maxRadius)
        {
            // Если враг слишком далеко, двигаемся к герою
            moveTowardsHero(direction);
            directionFront = true;
        }
        else if (distanceToHero < minRadius)
        {
            // Если враг слишком близко, отдаляемся от героя
            moveAwayFromHero(direction);
            directionFront =false;
        }
        else
        {
            moveCircle(distanceToHero, direction);
        }
    }

    private void moveTowardsHero(Vector2 direction)
    {
        // Двигаемся к герою
        rb.linearVelocity = direction * -moveSpeed;
    }

    private void moveAwayFromHero(Vector2 direction)
    {
        // Отдаляемся от героя
        rb.linearVelocity = direction * moveSpeed;
    }

    private void moveCircle(float distanceToHero, Vector2 directionVector)
    {
        
        
        // Увеличиваем таймер
        timer += Time.deltaTime;


        // Меняем направление каждую секунду
        if (timer >= changeInterval)
        {
            directionCircle *= -1; // Меняем направление на противоположное
            timer = 0f; // Сбрасываем таймер
            changeInterval = Random.Range(1, 5);
        }

        float angle = Mathf.Atan2(directionVector.y, directionVector.x);
        angle += (moveSpeed / 7f ) * directionCircle * Time.deltaTime;   // ТУТ ЕБАНЫЙ ХАРДКОД ДЛЯ УМЕНЬШЕНИЯ СКОРОСТИ 
        // Рассчитываем новую позицию
        float x = playerTransform.position.x + Mathf.Cos(angle) * distanceToHero;
        float y = playerTransform.position.y + Mathf.Sin(angle) * distanceToHero;

        // Направление для движения по кругу
        Vector2 targetPosition = new Vector2(x, y);
        Vector2 moveDirection = (targetPosition - rb.position).normalized;

        // Применяем скорость для движения по кругу
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}
