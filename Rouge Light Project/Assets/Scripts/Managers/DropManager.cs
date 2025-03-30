using System;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using log4net;
using Mono.Cecil.Cil;
using static UnityEngine.Rendering.DebugUI;

public class DropManager : MonoBehaviour
{
    //Добавляем логирование
    private static readonly ILog log = LogManager.GetLogger(typeof(DropManager));

    private static Hero heroScript;
    private static float luck; 

    public GameObject dropPrefab;
    private GameObject enemy;

    void Start()
    {
        // Получаем объект, к которому прикреплен этот скрипт
        enemy = gameObject;
    }

    public void DropLoss()
    {
        float diceRoll = Random.Range(0, 1f);

        GameObject heroObject = GameObject.Find("Player");
        // Получаем компонент Hero
        if (heroObject != null)
        {
            heroScript = heroObject.GetComponent<Hero>();
        }
        if (heroScript != null)
        {
            //Подтягиваем удачу из героя
            log.Debug("Переменная удачи из Hero: " + heroScript.heroStats.Luck);
            luck = 1f / (heroScript.heroStats.Luck);

            //Проверка удачи на выпадение дропа
            if (diceRoll < luck)
            {
                log.Debug("Удача на твоей стороне, выпадение дропа!");
                ChoosingDrop();

            }
            else
            {
                log.Debug("Удача не пройдена - дроп не выпадет :  diceRoll - " + diceRoll + " < luck - " + luck);
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
        TypeOfDrop dropCode;

        //Проверяем какой дроп выпадет
        if (diceRoll < 11)
        {
            log.Debug("Выпадает улучшение характеристики");
            dropCode = TypeOfDrop.TYPE_CHARACTER;
        }
        else if (diceRoll < 31)
        {
            log.Debug("Выпадает улучшение дота");
            dropCode = TypeOfDrop.TYPE_DOT;
        }
        else if (diceRoll < 61)
        {
            log.Debug("Выпадают деньги");
            dropCode = TypeOfDrop.TYPE_MONEY;
        }
        else
        {
            log.Debug("Выпадает хилка");
            dropCode = TypeOfDrop.TYPE_HEAL;
        }
        //Пока это условность, всегда будет выпадать улучшение характеристики
        createDrop(dropCode);
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
            default:
                log.Error($"Неизвестная тип дропа: {dropCode}");
                return;
        }



        if (dropPrefab is null) { log.Error("dropPrefab is null"); }
        if (enemy is null) { log.Error("createDrop - enemy is null"); }
        GameObject drop = Instantiate(dropPrefab, enemy.transform.position, Quaternion.identity);

        Drop dropScript = drop.GetComponent<Drop>();
        // Передаем дропу характеристики
        dropScript.DropCode = dropCode;
        dropScript.ItemCode = itemCode;
        dropScript.Update = 3; //ЕБАННЫЙ ХАРДКОД

        if (dropCode == TypeOfDrop.TYPE_DOT)
        {
            dropScript.IsDmgUpIfDot = Random.value > 0.5f;
        }

        // Получаем ссылку на дочерний объект "circle"
        Transform circleTransform = drop.transform.Find("Circle");

        if (circleTransform != null)
        {
            // Получаем SpriteRenderer из дочернего объекта
            SpriteRenderer dropSprite = circleTransform.GetComponent<SpriteRenderer>();

            if (dropSprite != null)
            {
                // Теперь можно установить цвет
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
        // Фильтруем объекты по type, характеристика или дот
        var filteredItems = ImprovableCharactesDictionary.GetAllImprovableCharacteristicsAndDots().Where(item => item.Type == type).ToList();

        // Если список пуст, возвращаем null
        if (filteredItems.Count == 0)
        {
            log.Error("Улучшаемый/ая " + (type ? "характеристика" : "дот") + " не найден, type = " + type);
            return null;
        }

        // Генерируем случайный индекс
        int randomIndex = Random.Range(0, filteredItems.Count);

        // Возвращаем code выбранного объекта
        log.Debug("Рандомно выбрали - " + filteredItems[randomIndex].Code);
        return filteredItems[randomIndex].Code;
    }
}
