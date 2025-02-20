using UnityEngine;

[System.Serializable]
public class DotEffect
{
    public string code; // Уникальный код эффекта
    public int finalDotDmg; // Финальный урон за тик
    public float finalDotDur; // Финальная длительность эффекта
    public int DmgUpgCount; // Количество улучшений урона
    public int DurUpgCount; // Количество улучшений длительности

    private float elapsedTime;
    private float nextTickTime;

    public bool IsFinished => elapsedTime >= finalDotDur;

    public DotEffect(string code, int finalDotDmg, float finalDotDur, int dmgUpgCount, int durUpgCount)
    {
        this.code = code;
        this.finalDotDmg = finalDotDmg;
        this.finalDotDur = finalDotDur;
        DmgUpgCount = dmgUpgCount;
        DurUpgCount = durUpgCount;
    }

    public void Tick(IDamageable target)
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= nextTickTime)
        {
            target.TakeDamage(finalDotDmg);
            nextTickTime += 1.0f; // Тик каждую секунду
        }
    }
}