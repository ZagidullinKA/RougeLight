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
            //Debug.Log("Наносим урон");
            bullet.DamageDealing();
            bullet.DestroyBullet();
        }
    }

    bool TagChecking(string otherTag)
    {
        bulletTag = bulletTag.Replace("Bullet", "");
        return string.Compare(otherTag, bulletTag) == 0 ? false : true;
    }


}
