using System;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class DotEffect
{
    public string code; // Уникальный код эффекта
    public int DotDmg; // Финальный урон за тик
    public float DotDur; // Финальная длительность эффекта
    public int DmgUpgCount; // Количество улучшений урона
    public int DurUpgCount; // Количество улучшений длительности
    public string affectedChar; // Количество улучшений длительности
    public string type; // Количество улучшений длительности

    private float elapsedTime;
    private float nextTickTime;

    public bool IsFinished => elapsedTime >= DotDur;

    public DotEffect(string code, int finalDotDmg, float finalDotDur, int dmgUpgCount, int durUpgCount)
    {
        this.code = code;
        this.DotDmg = finalDotDmg;
        this.DotDur = finalDotDur;
        this.DmgUpgCount = dmgUpgCount;
        this.DurUpgCount = durUpgCount;

        var dotItem = DotsDictionary.GetDot(code);
        ValidationValue.ValidateStringNotNullOrEmpty(
                        (dotItem.Type, "upgradableItem.Type"),
                        (dotItem.AffectedChar, "upgradableItem.AffectedChar")
                    );

        this.affectedChar = dotItem.AffectedChar;
        this.type = dotItem.Type;
    }

    public void Tick(IDamageable target)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= nextTickTime)
        {
            target.TakeDamage(DotDmg);
            nextTickTime += 1.0f; // Тик каждую секунду
        }
    }
}