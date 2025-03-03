using System;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using log4net;

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
            log.Debug("Переменная удачи из Hero: " + heroScript.Luck);
            luck = 1f / (heroScript.Luck);

            //Проверка удачи на выпадение дропа
            if (diceRoll < luck)
            {
                log.Debug("Удача на твоей стороне, выпадение дропа!");
                ChoosingDrop();

            } else
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

        //Проверяем какой дроп выпадет
        if (diceRoll < 11)
        {
            log.Debug("Выпадает улучшение характеристики");
        }
        else if (diceRoll < 51)
        {
            log.Debug("Выпадает улучшение дота");
        }
        else
        {
            log.Debug("Выпадают деньги");
        }
        //Пока это условность, всегда будет выпадать улучшение характеристики
        createDrop();
    }

    private void createDrop() {
        string code = getRandomCharacter();

        if (dropPrefab is null) { log.Error("dropPrefab is null"); }
        if (enemy is null) { log.Error("createDrop - enemy is null"); }
        GameObject drop = Instantiate(dropPrefab, enemy.transform.position, Quaternion.identity);


        Drop dropScript = drop.GetComponent<Drop>();
        // Передаем дропу характеристики
        dropScript.Code = code;
        dropScript.Update = 3;
    }

    private String getRandomCharacter() {
        // Фильтруем объекты, где type = true
        var filteredItems = ImprovableCharactesDictionary.GetAllImprovableCharacteristicsAndDots().Where(item => item.Type).ToList();

        // Если список пуст, возвращаем null
        if (filteredItems.Count == 0)
        {
            log.Error("Улучшаемая характеристика не найдена");
            return null;
        }

        // Генерируем случайный индекс
        int randomIndex = Random.Range(0, filteredItems.Count);

        // Возвращаем code выбранного объекта
        log.Debug("Рандомно выбрали - " + filteredItems[randomIndex].Code);
        return filteredItems[randomIndex].Code;
    }
}
