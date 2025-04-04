// Импорт необходимых пространств имен
using log4net;         // Для системы логирования
using UnityEngine;     // Базовые функции Unity

// Класс EnemyTestAI реализует ИИ поведения врага
public class EnemyTestAI : MonoBehaviour
{
    //Добавляем логирование
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(EnemyTestAI));

    // Параметры движения врага
    private Transform playerTransform;  // Ссылка на трансформ героя
    private float minRadius = 3f;       // Минимальный радиус (50 пикселей) - дистанция приближения к игроку
    private float maxRadius = 5f;       // Максимальный радиус (100 пикселей) - дистанция отдаления от игрока
    private float moveSpeed;            // Скорость движения врага

    // Свойство для доступа к скорости движения
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    // Переменные для управления движением
    private float timer = 0f;           // Таймер для смены направления
    private int directionCircle = 1;    // Направление движения (1 — по часовой стрелке, -1 — против)
    private bool directionFront = true; // Направление движения (true — к герою, false — от героя)
    float changeInterval;               // Текущий интервал смены направления
    Vector2 direction;                  // Текущее направление движения

    private Rigidbody2D rb; // Ссылка на компонент Rigidbody2D

    // Метод Start вызывается при инициализации объекта
    void Start()
    {
        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        // Получаем параметры моба из компонента Mobs
        Mobs mobsScript = gameObject.GetComponent<Mobs>();
        moveSpeed = mobsScript.mobStats.MoveSpeed;

        // Устанавливаем случайный интервал смены направления
        changeInterval = Random.Range(1, 5);
        log.Debug("Инициализация скорости передвижения - " + moveSpeed + " mobsScript.MoveSpeed - " + mobsScript.mobStats.MoveSpeed);

        // Инициализация компонента Rigidbody2D
        rb = GetComponent<Rigidbody2D>();

        // Настройка физики
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0; // Отключаем гравитацию
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Включаем непрерывное обнаружение коллизий
        rb.freezeRotation = true; // Запрещаем вращение
    }

    // Метод обработки столкновений
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            log.Debug("Столкновение с: " + collision.gameObject.name + " Tag - " + collision.gameObject.tag);

            // При столкновении с другим врагом
            if (collision.gameObject.CompareTag("Enemy"))
            {
                // Меняем направление движения по кругу
                directionCircle *= -1;

                // Рассчитываем направление отталкивания
                Vector2 pushDirection = (rb.position - (Vector2)collision.transform.position).normalized;

                // Применяем отталкивание через AddForce
                rb.AddForce(pushDirection * moveSpeed * 50f, ForceMode2D.Impulse); // Используем импульс для отталкивания
            }
        }
    }

    // Метод Update вызывается каждый кадр
    void Update()
    {
        // Проверка наличия ссылки на игрока
        if (playerTransform == null)
        {
            Debug.LogError("Герой не назначен!");
            return;
        }

        // Расстояние до героя
        float distanceToHero = Vector2.Distance(transform.position, playerTransform.position);
        direction = (transform.position - playerTransform.position).normalized;

        // Логика движения в зависимости от расстояния до героя
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
            directionFront = false;
        }
        else
        {
            // Двигаемся по кругу вокруг героя
            moveCircle(distanceToHero, direction);
        }
    }

    // Метод движения к герою
    private void moveTowardsHero(Vector2 direction)
    {
        // Двигаемся к герою (обратное направление)
        rb.linearVelocity = direction * -moveSpeed;
    }

    // Метод движения от героя
    private void moveAwayFromHero(Vector2 direction)
    {
        // Отдаляемся от героя
        rb.linearVelocity = direction * moveSpeed;
    }

    // Метод движения по кругу вокруг героя
    private void moveCircle(float distanceToHero, Vector2 directionVector)
    {
        // Увеличиваем таймер
        timer += Time.deltaTime;

        // Меняем направление по истечении интервала
        if (timer >= changeInterval)
        {
            directionCircle *= -1; // Меняем направление на противоположное
            timer = 0f; // Сбрасываем таймер
            changeInterval = Random.Range(1, 5); // Устанавливаем новый случайный интервал
        }

        // Рассчитываем угол для движения по кругу
        float angle = Mathf.Atan2(directionVector.y, directionVector.x);
        angle += (moveSpeed / 7f) * directionCircle * Time.deltaTime;   // ТУТ ЕБАНЫЙ ХАРДКОД ДЛЯ УМЕНЬШЕНИЯ СКОРОСТИ 

        // Рассчитываем новую позицию на окружности
        float x = playerTransform.position.x + Mathf.Cos(angle) * distanceToHero;
        float y = playerTransform.position.y + Mathf.Sin(angle) * distanceToHero;

        // Направление для движения по кругу
        Vector2 targetPosition = new Vector2(x, y);
        Vector2 moveDirection = (targetPosition - rb.position).normalized;

        // Применяем скорость для движения по кругу
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}