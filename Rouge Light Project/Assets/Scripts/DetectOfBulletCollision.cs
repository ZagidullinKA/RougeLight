using UnityEngine;

public class DetectOfBulletCollision : MonoBehaviour
{
    private string bulletTag;
    //public GameObject bullet;

    private Bullet bullet;
    private void Start()
    {
        bulletTag = gameObject.tag;
        bullet = gameObject.GetComponent<Bullet>();
        Debug.Log(bullet.name);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Есть контакт!");
        GameObject otherObject = collision.gameObject;
        string otherTag = otherObject.tag;
        if (TagChecking(otherTag))
        {
            Debug.Log("Проверка тегов пройдена");
            TestUnit testunit = otherObject.GetComponent<TestUnit>();
            if (testunit != null)
            {
                if (!testunit.TryDodge())
                {
                    // Вызываем метод TakeDamage и передаем урон
                    testunit.TakeDamage(bullet.DamageDealing());
                    bullet.DestroyBullet();
                }
                else Debug.Log("Уворот");
            }
            else
            {
                Debug.LogWarning("Компонент Enemy не найден на объекте: " + otherObject.name);
            }

        }
    }

    bool TagChecking(string otherTag)
    {
        bulletTag = bulletTag.Replace("Bullet", "");
        return string.Compare(otherTag, bulletTag) == 0 ? false : true;
    }


}
