using log4net;
using Unity.VisualScripting;
using UnityEngine;

public class Drop : MonoBehaviour
{
    // Логгер для отладки
    private static readonly ILog log = LogManager.GetLogger(typeof(Drop));

    private TypeOfDrop dropCode;
    private string itemCode;
    private int update;
    private bool? isDmgUpIfDot;

    public TypeOfDrop DropCode
    {
        get { return dropCode; }
        set { dropCode = value; }
    }

    public string ItemCode
    {
        get { return itemCode; }
        set { itemCode = value; }
    }

    public int Update
    {
        get { return update; }
        set { update = value; }
    }

    public bool? IsDmgUpIfDot
    {
        get { return isDmgUpIfDot; }
        set { isDmgUpIfDot = value; }
    }
}