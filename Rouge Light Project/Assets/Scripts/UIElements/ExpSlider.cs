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
    private static bool accsess = true;
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
        if (expQueue.Count > 0)
        {
            if (accsess)
            {
                log.Debug("Update. IncreasetExp.Начало обработки очереди.");
                accsess = false;
                QueueSlider itemQueue = expQueue.Dequeue();

                log.Debug("Update. IncreasetExp. Обработка элемента очереди: lvlCount = " + itemQueue.lvlCount 
                    + ", currentExp = " + itemQueue.currentExp 
                    + ", expNextLvl = " + string.Join(", ", itemQueue.expNextLvl)
                    + " currentExpSlider = " + currentExpSlider);

                if (itemQueue.lvlCount != 0)
                {
                    Sequence sequence = DOTween.Sequence();
                    Queue<float> maxValueQueue = new Queue<float>();

                    currentExpSlider = 0;


                    for (var i = 0; i < itemQueue.lvlCount; i++)
                    {
                        

                        if (itemQueue.expNextLvl == null || itemQueue.expNextLvl.Count == 0)
                        {
                            log.Error("Update. IncreasetExp. value.expNextLvl is null or empty.");
                            accsess = true;
                            return;
                        }

                        sequence.Append(expSlider.DOValue((maxExp + 0.1f), 2f).SetEase(Ease.InOutQuad)
                            .OnUpdate(() =>
                            {
                                UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);
                            }));

                        sequence.Append(expSlider.DOValue(0, 2f).SetEase(Ease.InOutQuad)
                        .OnUpdate(() =>
                        {
                            UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);

                        }).OnComplete(() =>
                        {
                            expSlider.maxValue = maxValueQueue.Dequeue();
                        }));


                        maxExp = itemQueue.expNextLvl[i] - maxExpPrev;
                        maxExpPrev = itemQueue.expNextLvl[i];
                        maxValueQueue.Enqueue(maxExp);
                    }
                    if (itemQueue.currentExp != 0)
                    {
                        sequence.Append(moveSliderToCurrentExp((int)itemQueue.currentExp));
                    }
                    sequence.OnComplete(() => {
                        accsess = true;
                            log.Debug("Update. IncreasetExp. Sequence завершён.");
                    });
                } else
                {
                    if (itemQueue.currentExp != 0)
                    {
                        Sequence moveSequence = DOTween.Sequence();
                        moveSequence.Append(moveSliderToCurrentExp((int)itemQueue.currentExp));
                        moveSequence.OnComplete(() =>
                        {
                            accsess = true;
                            log.Debug("Update. IncreasetExp. Анимация moveSliderToCurrentExp завершена");
                        });
                    }
                    else
                    {
                        accsess = true;
                    }
                }
                
            }
        }
    }


    private Tween moveSliderToCurrentExp(int changeCurrentExp)
    {
        return expSlider.DOValue(currentExpSlider + (int)changeCurrentExp, 1f).SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                UIManager.Instance.printExp((int)expSlider.value, (int)maxExp);
            })
            .OnComplete(() =>
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

        List<int> copyExpNextLvl = new List<int>(expNextLvl);
        expQueue.Enqueue(new QueueSlider(copyExpNextLvl, lvlCount, currentExp));
    }


    public static void setMaxExp( int expNextLvl)
    {
        maxExp = expNextLvl;
        maxExpPrev = expNextLvl;
        UIManager.Instance.printExp((int)currentExpSlider, (int)maxExp);
    }




    public static void TestAddExp()
    {
        Sequence sequence = DOTween.Sequence();
        Queue<float> maxValueQueue = new Queue<float>();
        float maxValueInteration = expSlider.maxValue;

        for (int i = 0; i < 5; i++)
        {
            sequence.Append(expSlider.DOValue((maxValueInteration + 0.1f), 2f).SetEase(Ease.InOutQuad)
                .OnUpdate(() =>
                {
                    UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);
                }));

            sequence.Append(expSlider.DOValue(0, 2f).SetEase(Ease.InOutQuad)
                .OnUpdate(() =>
                {
                    UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);
                    
                }).OnComplete(() =>
                {
                    expSlider.maxValue = maxValueQueue.Dequeue();
                }));
            maxValueInteration += 2;
            maxValueQueue.Enqueue(maxValueInteration);
        }

        sequence.OnComplete(() =>
        {
            maxValueQueue.Clear();
            UIManager.Instance.printExp((int)expSlider.value, (int)expSlider.maxValue);
            log.Debug("Update. IncreasetExpTest. Все анимации завершены.");
        });
    }

}

