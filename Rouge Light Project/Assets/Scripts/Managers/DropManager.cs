using System;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using log4net;
using Mono.Cecil.Cil;
using static UnityEngine.Rendering.DebugUI;

public class DropManager : MonoBehaviour
{
    // Логгер для отладки
    private static readonly ILog log = LogManager.GetLogger(typeof(DropManager));

    private static Hero heroScript;
    private static float luck; 

    public GameObject dropPrefab;
    private GameObject enemy;

    void Start()
    {
        // Получаем врага, в данном случае это сам объект
        enemy = gameObject;
    }

    public void DropLoss()
    {
        float diceRoll = Random.Range(0, 1f);

        GameObject heroObject = GameObject.Find("Player");
        // Получаем скрипт Hero
        if (heroObject != null)
        {
            heroScript = heroObject.GetComponent<Hero>();
        }
        if (heroScript != null)
        {
            // Вычисляем шанс на основе удачи
            log.Debug("Получена удача от Hero: " + heroScript.heroStats.Luck);
            luck = 1f / (heroScript.heroStats.Luck);

            // Проверяем шанс на выпадение дропа
            if (diceRoll < luck)
            {
                log.Debug("Шанс на дроп выпал, создаем дроп!");
                ChoosingDrop();

            }
            else
            {
                log.Debug("Шанс на дроп не выпал - дроп не создан :  diceRoll - " + diceRoll + " < luck - " + luck);
            }
        }
        else
        {
            log.Error("Скрипт Hero не найден!");
        }

    }

    private void ChoosingDrop()
    {
        float diceRoll = Random.Range(1, 101);

        // Определяем тип выпавшего дропа
        if (diceRoll < 11)
        {
            log.Debug("Выпал тип характеристика");
            createDrop(TypeOfDrop.TYPE_CHARACTER);
        }

        diceRoll = Random.Range(1, 101);
        if (diceRoll < 21)
        {
            log.Debug("Выпал тип дот");
            createDrop(TypeOfDrop.TYPE_DOT);
        }

        diceRoll = Random.Range(1, 101);
        if (diceRoll < 31)
        {
            log.Debug("Выпали деньги");
            createDrop(TypeOfDrop.TYPE_MONEY);
        }

        diceRoll = Random.Range(1, 101);
        if (diceRoll < 31) {
            log.Debug("Выпало зелье");
            createDrop(TypeOfDrop.TYPE_HEAL);
        }

        diceRoll = Random.Range(1, 101);
        if (diceRoll < 101) {
            
            createDrop(TypeOfDrop.TYPE_MODIFIER_ATTACK);
        }
        
    }

    private void createDrop(TypeOfDrop dropCode)
    {

        string itemCode;
        Color dropColor;
        switch (dropCode)
        {
            case TypeOfDrop.TYPE_CHARACTER:
                itemCode = getRandomCharacterOrDot(true);
                dropColor = Color.blue;
                break;
            case TypeOfDrop.TYPE_DOT:
                itemCode = getRandomCharacterOrDot(false);
                dropColor = Color.green;
                break;
            case TypeOfDrop.TYPE_MONEY:
                itemCode = TypeOfDrop.TYPE_MONEY.ToString();
                dropColor = Color.yellow;
                break;
            case TypeOfDrop.TYPE_HEAL:
                itemCode = TypeOfDrop.TYPE_HEAL.ToString();
                dropColor = Color.red;
                break;
            case TypeOfDrop.TYPE_MODIFIER_ATTACK:
                int lvl = ShootingModifiersDictionary.GetRandomLevel();
                itemCode = ShootingModifiersDictionary.GetRandomModifierCodeByLevel(lvl);
                log.Debug("Выпал Модификатор атаки: " + itemCode);
                dropColor = new Color(0.5f, 0f, 0.5f); // Фиолетовый цвет
                break;
            default:
                log.Error($"Неизвестный тип дропа: {dropCode}");
                return;
        }



        if (dropPrefab is null) { log.Error("dropPrefab is null"); }
        if (enemy is null) { log.Error("createDrop - enemy is null"); }
        GameObject drop = Instantiate(dropPrefab, enemy.transform.position, Quaternion.identity);

        Drop dropScript = drop.GetComponent<Drop>();
        // Устанавливаем код дропа
        dropScript.DropCode = dropCode;
        dropScript.ItemCode = itemCode;
        dropScript.Update = 3; // Уровень улучшения

        if (dropCode == TypeOfDrop.TYPE_DOT)
        {
            dropScript.IsDmgUpIfDot = Random.value > 0.5f;
        }

        // Ищем дочерний объект с именем "circle"
        Transform circleTransform = drop.transform.Find("Circle");

        if (circleTransform != null)
        {
            // Получаем SpriteRenderer на найденном объекте
            SpriteRenderer dropSprite = circleTransform.GetComponent<SpriteRenderer>();

            if (dropSprite != null)
            {
                // Устанавливаем цвет спрайта дропа
                dropSprite.color = dropColor;
            }
            else
            {
                log.Error("SpriteRenderer не найден");
            }
        }
        else
        {
            log.Error("Дочерний объект 'circle' не найден");
        }
    }

    private String getRandomCharacterOrDot(bool type)
    {
        // Фильтруем предметы по type, характеристики или доты
        var filteredItems = ImprovableCharactesDictionary.GetAllImprovableCharacteristicsAndDots().Where(item => item.Type == type).ToList();

        // Если список пуст, возвращаем null
        if (filteredItems.Count == 0)
        {
            log.Error("Характеристики/доты не найдены, type = " + type);
            return null;
        }

        // Выбираем случайный индекс
        int randomIndex = Random.Range(0, filteredItems.Count);

        // Возвращаем code выбранного предмета
        log.Debug("Выбран предмет - " + filteredItems[randomIndex].Code);
        return filteredItems[randomIndex].Code;
    }
}