using log4net;
using UnityEngine;

public class EnemyTestAI : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(EnemyTestAI));

    private Transform playerTransform; // Ссылка на героя
    private float minRadius = 3f; // Минимальный радиус (50 пикселей)
    private float maxRadius = 5f; // Максимальный радиус (100 пикселей)
    private float moveSpeed; // Скорость движения врага
    private float timer = 0f; // Направление движения
    private int direction = 1; // Направление движения (1 — по часовой стрелке, -1 — против)
    float changeInterval; // Текущий интервал смены направления

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
        Mobs mobsScript = gameObject.GetComponent<Mobs>();
        moveSpeed = mobsScript.MoveSpeed;
        changeInterval = Random.Range(1, 5);
        log.Debug("Инициализация скорости передвижения - " + moveSpeed + " mobsScript.MoveSpeed - " + mobsScript.MoveSpeed);
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
        Vector2 direction = (transform.position - playerTransform.position).normalized;

        // Логика движения
        if (distanceToHero > maxRadius)
        {
            // Если враг слишком далеко, двигаемся к герою
            moveTowardsHero(direction);
        }
        else if (distanceToHero < minRadius)
        {
            // Если враг слишком близко, отдаляемся от героя
            moveAwayFromHero(direction);
        }
        else
        {
            moveCircle(distanceToHero, direction);
        }
    }

    private void moveTowardsHero(Vector2 direction)
    {
        // Двигаемся к герою
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
    }

    private void moveAwayFromHero(Vector2 direction)
    {
        // Отдаляемся от героя
        transform.position = Vector2.MoveTowards(transform.position, transform.position + (Vector3)direction, moveSpeed * Time.deltaTime);
    }

    private void moveCircle(float distanceToHero, Vector2 directionVector)
    {
        
        
        // Увеличиваем таймер
        timer += Time.deltaTime;


        // Меняем направление каждую секунду
        if (timer >= changeInterval)
        {
            direction *= -1; // Меняем направление на противоположное
            timer = 0f; // Сбрасываем таймер
            changeInterval = Random.Range(1, 5);
        }

        float angle = Mathf.Atan2(directionVector.y, directionVector.x);
        angle += (moveSpeed / 7f ) * direction * Time.deltaTime;   // ТУТ ЕБАНЫЙ ХАРДКОД ДЛЯ УМЕНЬШЕНИЯ СКОРОСТИ 
        // Рассчитываем новую позицию
        float x = playerTransform.position.x + Mathf.Cos(angle) * distanceToHero;
        float y = playerTransform.position.y + Mathf.Sin(angle) * distanceToHero;

        // Применяем новую позицию
        transform.position = new Vector2(x, y);
    }
}
