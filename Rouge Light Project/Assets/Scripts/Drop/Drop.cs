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
}
