using log4net;
using System.Linq;
using UnityEngine;

public class DetectOfBulletCollision : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(DetectOfBulletCollision));

    private string bulletTag;
    //public GameObject bullet;

    private Bullet bullet;
    private void Start()
    {
        bulletTag = gameObject.tag;
        bullet = gameObject.GetComponent<Bullet>();
        log.Debug(bullet.name);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject otherObject = collision.gameObject;
        string otherTag = otherObject.tag;
        if (TagChecking(otherTag))
        {
            log.Debug("Попал в противника");
            Character character = otherObject.GetComponent<Character>();
            if (character != null)
            {
                if (character.TryDodge())
                {
                    // Вызываем метод TakeDamage и передаем урон
                    character.TakeDamage(bullet.DamageDealing());
                    character.TakeDots(bullet.GetUsableDotsArray());
                    bullet.DestroyBullet();
                }
                else log.Debug("Уворот");
            }
            else
            {
                log.Warn("Скрипт не найден на объекте: " + otherObject.name);
            }

        }
        else
        {
            log.Debug("Попадание по своему");
        }

        bool TagChecking(string otherTag)
        {
            bulletTag = bulletTag.Replace("Bullet", "");
            return string.Compare(otherTag, bulletTag) == 0 ? false : true;
        }


    }
}
