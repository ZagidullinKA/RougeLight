// Импорт необходимых пространств имен
using System.Collections;       // Для работы с корутинами (не используется в текущем коде)
using System.Collections.Generic; // Для работы с коллекциями (не используется в текущем коде)
using UnityEngine;              // Базовые функции Unity

// Класс CameraController управляет поведением камеры, следуя за игроком
public class CameraController : MonoBehaviour
{
    // Ссылка на трансформ игрока, за которым должна следовать камера
    public Transform player;

    // Метод FixedUpdate вызывается с фиксированной частотой (по умолчанию 50 раз в секунду)
    // Используется для физических расчетов и плавного перемещения камеры
    void FixedUpdate()
    {
        // Создаем временную переменную для позиции камеры
        Vector3 temp = transform.position;

        // Устанавливаем X-координату камеры равной X-координате игрока
        temp.x = player.position.x;

        // Устанавливаем Y-координату камеры равной Y-координате игрока
        temp.y = player.position.y;

        // Применяем новую позицию к камере
        transform.position = temp;
    }
}