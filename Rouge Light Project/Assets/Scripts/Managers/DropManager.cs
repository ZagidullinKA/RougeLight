// Импорт необходимых пространств имен
using System;           // Базовые типы .NET
using UnityEngine;      // Базовые функции Unity
using System.Linq;      // Для работы с LINQ
using Random = UnityEngine.Random; // Псевдоним для Random
using log4net;          // Для системы логирования
using Mono.Cecil.Cil;   // Для работы с IL-кодом (не используется в текущем коде)
using static UnityEngine.Rendering.DebugUI; // Для доступа к DebugUI (не используется в текущем коде)

// Класс DropManager управляет выпадением дропа при смерти врагов
public class DropManager : MonoBehaviour
{
    //Добавляем логирование
    // Инициализация логгера для этого класса
    private static readonly ILog log = LogManager.GetLogger(typeof(DropManager));

    // Ссылки на компоненты и переменные
    private static Hero heroScript; // Ссылка на скрипт героя
    private static float luck;     // Текущее значение удачи

    public GameObject dropPrefab;  // Префаб для дропа
    private GameObject enemy;      // Ссылка на объект врага

    // Метод Start вызывается при инициализации объекта
    void Start()
    {
        // Получаем объект, к которому прикреплен этот скрипт
        enemy = gameObject;
    }

    // Основной метод обработки выпадения дропа
    public void DropLoss()
    {
        // Генерируем случайное число для проверки удачи
        float diceRoll = Random.Range(0, 1f);

        // Находим объект игрока
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
            // Рассчитываем шанс выпадения дропа (обратно пропорционально удаче)
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

    // Метод выбора типа дропа
    private void ChoosingDrop()
    {
        // Генерация случайного числа для определения типа дропа
        float diceRoll = Random.Range(1, 101);
        TypeOfDrop dropCode;

        //Проверяем какой дроп выпадет
        if (diceRoll < 11) // 10% шанс
        {
            log.Debug("Выпадает улучшение характеристики");
            dropCode = TypeOfDrop.TYPE_CHARACTER;
        }
        else if (diceRoll < 31) // 20% шанс (10-30)
        {
            log.Debug("Выпадает улучшение дота");
            dropCode = TypeOfDrop.TYPE_DOT;
        }
        else if (diceRoll < 61) // 30% шанс (30-60)
        {
            log.Debug("Выпадают деньги");
            dropCode = TypeOfDrop.TYPE_MONEY;
        }
        else // 40% шанс (60-100)
        {
            log.Debug("Выпадает хилка");
            dropCode = TypeOfDrop.TYPE_HEAL;
        }

        // Создаем выбранный дроп
        createDrop(dropCode);
    }

    // Метод создания дропа
    private void createDrop(TypeOfDrop dropCode)
    {
        string itemCode;
        Color dropColor;

        // Выбираем код предмета и цвет в зависимости от типа дропа
        switch (dropCode)
        {
            case TypeOfDrop.TYPE_CHARACTER:
                itemCode = getRandomCharacterOrDot(true); // Получаем случайную характеристику
                dropColor = Color.blue;                  // Синий цвет для характеристик
                break;
            case TypeOfDrop.TYPE_DOT:
                itemCode = getRandomCharacterOrDot(false); // Получаем случайный дот
                dropColor = Color.green;                  // Зеленый цвет для дотов
                break;
            case TypeOfDrop.TYPE_MONEY:
                itemCode = TypeOfDrop.TYPE_MONEY.ToString();
                dropColor = Color.yellow;                 // Желтый цвет для денег
                break;
            case TypeOfDrop.TYPE_HEAL:
                itemCode = TypeOfDrop.TYPE_HEAL.ToString();
                dropColor = Color.red;                    // Красный цвет для хилок
                break;
            default:
                log.Error($"Неизвестная тип дропа: {dropCode}");
                return;
        }

        // Проверка наличия префаба и объекта врага
        if (dropPrefab is null) { log.Error("dropPrefab is null"); }
        if (enemy is null) { log.Error("createDrop - enemy is null"); }

        // Создаем экземпляр дропа на позиции врага
        GameObject drop = Instantiate(dropPrefab, enemy.transform.position, Quaternion.identity);

        // Настраиваем параметры дропа
        Drop dropScript = drop.GetComponent<Drop>();
        // Передаем дропу характеристики
        dropScript.DropCode = dropCode;
        dropScript.ItemCode = itemCode;
        dropScript.Update = 3; //ЕБАННЫЙ ХАРДКОД (значение улучшения)

        // Для дотов случайно выбираем, улучшать ли урон
        if (dropCode == TypeOfDrop.TYPE_DOT)
        {
            dropScript.IsDmgUpIfDot = Random.value > 0.5f;
        }

        // Находим дочерний объект "circle" для изменения цвета
        Transform circleTransform = drop.transform.Find("Circle");

        if (circleTransform != null)
        {
            // Получаем SpriteRenderer из дочернего объекта
            SpriteRenderer dropSprite = circleTransform.GetComponent<SpriteRenderer>();

            if (dropSprite != null)
            {
                // Устанавливаем цвет дропа
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

    // Метод для получения случайной характеристики или дота
    private String getRandomCharacterOrDot(bool type)
    {
        // Фильтруем объекты по type (true - характеристики, false - доты)
        var filteredItems = ImprovableCharactesDictionary.GetAllImprovableCharacteristicsAndDots()
                           .Where(item => item.Type == type).ToList();

        // Если список пуст, возвращаем null
        if (filteredItems.Count == 0)
        {
            log.Error("Улучшаемый/ая " + (type ? "характеристика" : "дот") + " не найден, type = " + type);
            return null;
        }

        // Выбираем случайный элемент из отфильтрованного списка
        int randomIndex = Random.Range(0, filteredItems.Count);

        // Возвращаем code выбранного объекта
        log.Debug("Рандомно выбрали - " + filteredItems[randomIndex].Code);
        return filteredItems[randomIndex].Code;
    }
}