using log4net;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using System.Collections;

public class ExpSlider : MonoBehaviour
{
    private static readonly ILog log = LogManager.GetLogger(typeof(ExpSlider));

    private static Slider expSlider;
    private static float maxExp = 0;
    private static float maxExpPrev = 0;
    private static float currentExpSlider = 0;
    private static bool AccessToAnimation = true;
    private static Queue<QueueSlider> expQueue = new Queue<QueueSlider>();

    public class QueueSlider
    {
        public  List<int> expNextLvl;
        public  int lvlCount;
        public  int currentExp;

        public QueueSlider(List<int> expNextLvl, int lvlCount, int currentExp)
        {
            
            this.expNextLvl = expNextLvl;
            this.lvlCount = lvlCount;
            this.currentExp = currentExp;
        }
    }

    void Start()
    {
        GameObject sliderObject = GameObject.Find("ExpSlider");
        expSlider = sliderObject.GetComponent<Slider>();
        if (expSlider == null)
        {
            log.Error("expSlider не назначен!");
            return;
        }

        expSlider.maxValue = maxExp;
        expSlider.value = currentExpSlider;
        UIManager.Instance.printExp((int)currentExpSlider, (int) maxExp);
    }

    private void Update()
    {
        if (expQueue.Count > 0)             // Есть ли что-то в очереди анимации
        {
            if (AccessToAnimation)                    // Проверяем можно ли проводить анимацию (Не проводится ли другая?)
            {
                log.Debug("Update. IncreasetExp.Начало обработки очереди.");
                AccessToAnimation = false;                                  // Закрываем доступ к анимации
                QueueSlider itemQueue = expQueue.Dequeue();                 // Берем первый элемент из очереди

                log.Debug("Update. IncreasetExp. Обработка элемента очереди: lvlCount = " + itemQueue.lvlCount 
                    + ", currentExp = " + itemQueue.currentExp 
                    + ", expNextLvl = " + string.Join(", ", itemQueue.expNextLvl)
                    + " currentExpSlider = " + currentExpSlider);

                if (itemQueue.lvlCount != 0)                                // Повышаем уровень?
                {
                    Sequence sequence = DOTween.Sequence();                 // Создаем очередь анимации
                    Queue<float> maxValueQueue = new Queue<float>();        // Создаем очередь максимального exp, для кореектного вывода в каждом моменте
                    currentExpSlider = 0;                                   // Нынешнее значение exp уводим в 0


                    for (var i = 0; i < itemQueue.lvlCount; i++)            // Перебираем все повышения уровня
                    {
                        

                        if (itemQueue.expNextLvl == null || itemQueue.expNextLvl.Count == 0)        
                        {
                            log.Error("Update. IncreasetExp. value.expNextLvl is null or empty.");
                            AccessToAnimation = true;
                            return;
                        }

                        sequence.Append(expSlider.DOValue((maxExp + 0.1f), 2f).SetEase(Ease.InOutQuad)          // Добавляем в очередь анимацию увечеления слайдера до макс значения
                            .OnUpdate(() =>                                                                     // Что происходит пока, анимация работает
                            {
                                UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);
                            }));

                        sequence.Append(expSlider.DOValue(0, 2f).SetEase(Ease.InOutQuad)                        // Добавляем в очередь анимацию уменьшения значения слайдера до 0
                        .OnUpdate(() =>                                                                         // Что происходит пока, анимация работает
                        {
                            UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);

                        }).OnComplete(() =>                                                                     // По выполнению анимации :
                        {
                            // Берем первый элемент очереди максимального exp и устанавливаем новое значение для слайдера
                            expSlider.maxValue = maxValueQueue.Dequeue();                                       
                            
                        }));


                        maxExp = itemQueue.expNextLvl[i] - maxExpPrev;          // Высчитываем какое количество exp нужно до след уровня
                        maxExpPrev = itemQueue.expNextLvl[i];                   // Запоминаем предыдущее количество exp до след уровня
                        maxValueQueue.Enqueue(maxExp);                          // Добавляем в очередь необходимое количество exp до след уровня
                    }
                    if (itemQueue.currentExp != 0)
                    {
                        sequence.Append(moveSliderToCurrentExp((int)itemQueue.currentExp));         // Добавляем в очередь анимацию увечелечения опыта до значения остатка после повышения уровней
                    }
                    sequence.OnComplete(() => {                     // По завершению всех анимаций
                        AccessToAnimation = true;                   // Открываем доступ к анимации
                        log.Debug("Update. IncreasetExp. Sequence завершён.");
                    });
                } else
                {
                    if (itemQueue.currentExp != 0)
                    {
                        Sequence moveSequence = DOTween.Sequence();     // Создаем очередь анимации
                        moveSequence.Append(moveSliderToCurrentExp((int)itemQueue.currentExp)); // Добавляем в очередь анимацию увечелечения опыта до нового значения опыта
                        moveSequence.OnComplete(() =>                   // По завершению всех анимаций
                        {
                            AccessToAnimation = true;                   // Открываем доступ к анимации
                            log.Debug("Update. IncreasetExp. Анимация moveSliderToCurrentExp завершена");
                        });
                    }
                    else
                    {
                        AccessToAnimation = true;   // Открываем доступ к анимации
                    }
                }
                
            }
        }
    }


    private Tween moveSliderToCurrentExp(int changeCurrentExp)
    {
        return expSlider.DOValue(currentExpSlider + (int)changeCurrentExp, 1f).SetEase(Ease.Linear)         // Создаем анимацию по увеличению опытаот нынешнего положения сладера до нового
            .OnUpdate(() =>                                         // Что происходит пока, анимация работает
            {
                UIManager.Instance.printExp((int)expSlider.value, (int)maxExp);
            })
            .OnComplete(() =>                                       // По завершению этой анимации
            {
                currentExpSlider += (int)changeCurrentExp;
                log.Debug("Update. IncreasetExp. moveSliderToCurrentExp: Отображаем в цифрах currentExpSlider = " + currentExpSlider);
                UIManager.Instance.printExp((int)currentExpSlider, (int)maxExp);
            });
    }

    public static void AddExp(List<int> expNextLvl, int lvlCount, int currentExp)
    {
        log.Debug("AddExp. IncreasetExp. expNextLvl = " 
            + string.Join(", ", expNextLvl) + " lvlCount = " 
            + lvlCount + " currentExp = " 
            + currentExp
        );


        // Добавляем данные в очередь обработки анимации
        List<int> copyExpNextLvl = new List<int>(expNextLvl);
        expQueue.Enqueue(new QueueSlider(copyExpNextLvl, lvlCount, currentExp));
    }


    public static void setMaxExp( int expNextLvl)
    {
        // Метод для первоначальной установки exp

        maxExp = expNextLvl;
        maxExpPrev = expNextLvl;
        UIManager.Instance.printExp((int)currentExpSlider, (int)maxExp);
    }
}

