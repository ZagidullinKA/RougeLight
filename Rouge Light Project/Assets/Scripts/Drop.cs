using log4net;
using Unity.VisualScripting;
using UnityEngine;

public class Drop : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Drop));

    private string code;
    private int update;

    public string Code
    {
        get { return code; }
        set { code = value; }
    }

    public int Update
    {
        get { return update; }
        set { update = value; }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        log.Debug("Столкновение с дропом!");
        GameObject otherObject = collision.gameObject;
        string otherTag = otherObject.tag;
        if (otherTag == "Player")
        {
            log.Debug("Предмет подобрали!");
            Hero hero = otherObject.GetComponent<Hero>();
            if (hero != null)
            {
                hero.SetCharacteristic(code, update);
                Destroy(this.gameObject);
                log.Debug("Дроп уничтожен");
            }
        }
        else
        {
            log.Debug("Кто-то другой столкнулся с дропом");
        }
    }
}
