using log4net;
using Unity.VisualScripting;
using UnityEngine;

// Класс Drop представляет выпадающие предметы в игре
// Отвечает за хранение и передачу данных о подбираемых предметах
public class Drop : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(Drop));

    // Тип дропа (определяет категорию предмета)
    private TypeOfDrop dropCode;
    // Код конкретного предмета/эффекта
    private string itemCode;
    // Значение улучшения/изменения, которое дает предмет
    private int update;
    // Флаг для DoT-эффектов: true - улучшает урон, false/null - улучшает длительность
    private bool? isDmgUpIfDot;

    // Свойство для доступа к типу дропа
    public TypeOfDrop DropCode
    {
        get { return dropCode; }
        set { dropCode = value; }
    }

    // Свойство для доступа к коду предмета
    public string ItemCode
    {
        get { return itemCode; }
        set { itemCode = value; }
    }

    // Свойство для доступа к значению улучшения
    public int Update
    {
        get { return update; }
        set { update = value; }
    }

    // Свойство для определения типа улучшения DoT-эффекта
    public bool? IsDmgUpIfDot
    {
        get { return isDmgUpIfDot; }
        set { isDmgUpIfDot = value; }
    }
}