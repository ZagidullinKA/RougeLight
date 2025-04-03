using log4net;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class ExpSlider : MonoBehaviour
{
    // Логгер для записи событий и ошибок
    private static readonly ILog log = LogManager.GetLogger(typeof(ExpSlider));

    // Ссылка на компонент Slider для отображения опыта
    private static Slider expSlider;
    // Максимальное количество опыта для текущего уровня
    private static float maxExp = 0;
    // Предыдущее максимальное значение опыта (для расчетов при повышении уровня)
    private static float maxExpPrev = 0;
    // Текущее значение опыта на слайдере
    private static float currentExpSlider = 0;
    // Флаг, разрешающий/запрещающий выполнение анимации
    private static bool AccessToAnimation = true;
    // Очередь анимаций для обработки опыта и уровней
    private static Queue<QueueSlider> expQueue = new Queue<QueueSlider>();

    // Вложенный класс для хранения данных об опыте и уровнях в очереди
    public class QueueSlider
    {
        public List<int> expNextLvl;  // Список значений опыта для следующих уровней
        public int lvlCount;          // Количество повышений уровня
        public int currentExp;         // Текущее количество опыта

        public QueueSlider(List<int> expNextLvl, int lvlCount, int currentExp)
        {
            this.expNextLvl = expNextLvl;
            this.lvlCount = lvlCount;
            this.currentExp = currentExp;
        }
    }

    void Start()
    {
        // Поиск слайдера опыта на сцене
        GameObject sliderObject = GameObject.Find("ExpSlider");
        expSlider = sliderObject.GetComponent<Slider>();
        if (expSlider == null)
        {
            log.Error("expSlider не назначен!");
            return;
        }

        // Установка начальных значений слайдера
        expSlider.maxValue = maxExp;
        expSlider.value = currentExpSlider;
        // Обновление UI с текущими значениями опыта
        UIManager.Instance.printExp((int)currentExpSlider, (int)maxExp);
    }

    private void Update()
    {
        // Проверяем наличие элементов в очереди анимаций
        if (expQueue.Count > 0)             // Есть ли что-то в очереди анимации
        {
            // Проверяем, можно ли начать новую анимацию
            if (AccessToAnimation)                    // Проверяем можно ли проводить анимацию (Не проводится ли другая?)
            {
                log.Debug("Update. IncreasetExp.Начало обработки очереди.");
                AccessToAnimation = false;                                  // Блокируем выполнение других анимаций
                QueueSlider itemQueue = expQueue.Dequeue();                 // Извлекаем первый элемент из очереди

                log.Debug("Update. IncreasetExp. Обработка элемента очереди: lvlCount = " + itemQueue.lvlCount
                    + ", currentExp = " + itemQueue.currentExp
                    + ", expNextLvl = " + string.Join(", ", itemQueue.expNextLvl)
                    + " currentExpSlider = " + currentExpSlider);

                // Если нужно повысить уровень
                if (itemQueue.lvlCount != 0)                                // Повышаем уровень?
                {
                    // Создаем последовательность анимаций
                    Sequence sequence = DOTween.Sequence();                 // Создаем очередь анимации
                    Queue<float> maxValueQueue = new Queue<float>();        // Очередь для хранения максимальных значений опыта
                    currentExpSlider = 0;                                   // Сбрасываем текущее значение опыта

                    // Обрабатываем каждое повышение уровня
                    for (var i = 0; i < itemQueue.lvlCount; i++)            // Перебираем все повышения уровня
                    {
                        // Проверка на наличие данных о следующем уровне
                        if (itemQueue.expNextLvl == null || itemQueue.expNextLvl.Count == 0)
                        {
                            log.Error("Update. IncreasetExp. value.expNextLvl is null or empty.");
                            AccessToAnimation = true;
                            return;
                        }

                        // Анимация заполнения шкалы опыта до максимума
                        sequence.Append(expSlider.DOValue((maxExp + 0.1f), 2f).SetEase(Ease.InOutQuad)
                            .OnUpdate(() =>
                            {
                                // Обновление UI во время анимации
                                UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);
                            }));

                        // Анимация сброса шкалы опыта после достижения максимума
                        sequence.Append(expSlider.DOValue(0, 2f).SetEase(Ease.InOutQuad)
                        .OnUpdate(() =>
                        {
                            UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);

                        }).OnComplete(() =>
                        {
                            // Устанавливаем новое максимальное значение опыта для следующего уровня
                            expSlider.maxValue = maxValueQueue.Dequeue();

                        }));

                        // Расчет новых значений опыта
                        maxExp = itemQueue.expNextLvl[i] - maxExpPrev;          // Разница между новым и старым максимумом
                        maxExpPrev = itemQueue.expNextLvl[i];                   // Сохраняем новый максимум
                        maxValueQueue.Enqueue(maxExp);                          // Добавляем в очередь для анимации
                    }

                    // Если остался неучтенный опыт после повышения уровней
                    if (itemQueue.currentExp != 0)
                    {
                        sequence.Append(moveSliderToCurrentExp((int)itemQueue.currentExp));
                    }

                    // Завершающие действия после всех анимаций
                    sequence.OnComplete(() => {
                        AccessToAnimation = true;                   // Разблокируем выполнение анимаций
                        log.Debug("Update. IncreasetExp. Sequence завершён.");
                    });
                }
                else
                {
                    // Если не было повышения уровня, но нужно изменить текущий опыт
                    if (itemQueue.currentExp != 0)
                    {
                        Sequence moveSequence = DOTween.Sequence();
                        moveSequence.Append(moveSliderToCurrentExp((int)itemQueue.currentExp));
                        moveSequence.OnComplete(() =>
                        {
                            AccessToAnimation = true;
                            log.Debug("Update. IncreasetExp. Анимация moveSliderToCurrentExp завершена");
                        });
                    }
                    else
                    {
                        AccessToAnimation = true;   // Разблокируем выполнение анимаций
                    }
                }

            }
        }
    }

    // Метод для анимации изменения текущего опыта без повышения уровня
    private Tween moveSliderToCurrentExp(int changeCurrentExp)
    {
        return expSlider.DOValue(currentExpSlider + (int)changeCurrentExp, 1f).SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                UIManager.Instance.printExp((int)expSlider.value, (int)maxExp);
            })
            .OnComplete(() =>
            {
                currentExpSlider += (int)changeCurrentExp;  // Обновляем текущее значение опыта
                log.Debug("Update. IncreasetExp. moveSliderToCurrentExp: Отображаем в цифрах currentExpSlider = " + currentExpSlider);
                UIManager.Instance.printExp((int)currentExpSlider, (int)maxExp);
            });
    }

    // Статический метод для добавления опыта в очередь обработки
    public static void AddExp(List<int> expNextLvl, int lvlCount, int currentExp)
    {
        log.Debug("AddExp. IncreasetExp. expNextLvl = "
            + string.Join(", ", expNextLvl) + " lvlCount = "
            + lvlCount + " currentExp = "
            + currentExp
        );

        // Создаем копию списка для безопасности
        List<int> copyExpNextLvl = new List<int>(expNextLvl);
        // Добавляем новые данные в очередь
        expQueue.Enqueue(new QueueSlider(copyExpNextLvl, lvlCount, currentExp));
    }

    // Метод для первоначальной установки максимального значения опыта
    public static void setMaxExp(int expNextLvl)
    {
        maxExp = expNextLvl;
        maxExpPrev = expNextLvl;
        UIManager.Instance.printExp((int)currentExpSlider, (int)maxExp);
    }
}