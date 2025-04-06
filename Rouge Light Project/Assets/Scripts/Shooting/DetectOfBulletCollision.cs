// Импорт необходимых пространств имен
using log4net;     // Для системы логирования
using System.Linq;  // Для работы с LINQ (не используется в текущем коде)
using UnityEngine;  // Базовые функции Unity

// Класс DetectOfBulletCollision отвечает за обработку столкновений пуль с объектами
public class DetectOfBulletCollision : MonoBehaviour
{
    //Добавляем логирование
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(DetectOfBulletCollision));

    private string bulletTag; // Тег пули (для идентификации принадлежности)
    //public GameObject bullet; // Закомментированная ссылка на объект пули

    private Bullet bullet; // Ссылка на компонент Bullet текущей пули

    // Метод Start вызывается при инициализации объекта
    private void Start()
    {
        // Запоминаем тег пули и получаем компонент Bullet
        bulletTag = gameObject.tag;
        bullet = gameObject.GetComponent<Bullet>();
    }

    // Метод обработки столкновений 2D объектов
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Получаем объект, с которым столкнулась пуля
        GameObject otherObject = collision.gameObject;
        string otherTag = otherObject.tag;

        // Проверяем, можно ли наносить урон этому объекту
        if (TagChecking(otherTag))
        {
            log.Debug("Попал в " + otherTag);

            // Пытаемся получить компонент Character у цели
            Character character = otherObject.GetComponent<Character>();
            if (character != null)
            {
                // Проверяем, удалось ли уклониться
                if (character.TryDodge())
                {
                    // Если уклон не удался:
                    // 1. Рассчитываем урон с учетом брони
                    character.CalculateDamageAfterArmor(bullet.DamageDealing(), TypeOfDamage.TYPE_ATTACK);
                    // 2. Применяем DOT-эффекты
                    character.TakeDots(bullet.GetUsableDotsArray());
                    // 3. Уничтожаем пулю
                    bullet.DestroyBullet();
                }
                else
                {
                    log.Debug("Уворот");
                }
            }
            else
            {
                log.Error("Скрипт не найден на объекте: " + otherObject.name);
            }
        }
        else
        {
            log.Warn("Попадание по своему"); // Пуля попала в объект своей команды
        }

        // Вложенный метод проверки тегов на возможность нанесения урона
        bool TagChecking(string otherTag)
        {
            // Удаляем "Bullet" из тега пули для получения базового тега (Player/Enemy)
            bulletTag = bulletTag.Replace("Bullet", "");
            // Сравниваем теги - возвращаем false если теги совпадают (свой объект)
            return string.Compare(otherTag, bulletTag) == 0 ? false : true;
        }
    }
}