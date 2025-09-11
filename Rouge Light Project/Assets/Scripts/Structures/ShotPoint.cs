using UnityEngine;

[System.Serializable]
public class ShotPoint
{
    [SerializeField] public float angle = 0f;           // Угол стрельбы в градусах
    [SerializeField] public Vector2 direction = Vector2.up; // Направление стрельбы
    [SerializeField] public bool isActive = true;       // Активна ли точка стрельбы

    public ShotPoint()
    {
        angle = 0f;
        direction = Vector2.up;
        isActive = true;
    }

    public ShotPoint(float angle, Vector2 direction, bool isActive = true)
    {
        this.angle = angle;
        this.direction = direction;
        this.isActive = isActive;
    }

    // Обновляет направление на основе угла
    public void UpdateDirectionFromAngle()
    {
        float angleRad = angle * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

    // Обновляет угол на основе направления
    public void UpdateAngleFromDirection()
    {
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    // Возвращает направление с учетом поворота объекта
    public Vector2 GetRotatedDirection(float objectRotationRad)
    {
        float cos = Mathf.Cos(objectRotationRad);
        float sin = Mathf.Sin(objectRotationRad);
        
        return new Vector2(
            direction.x * cos - direction.y * sin,
            direction.x * sin + direction.y * cos
        );
    }

    // Возвращает угол с учетом поворота объекта
    public float GetRotatedAngle(float objectRotationDeg)
    {
        return angle + objectRotationDeg;
    }
}
