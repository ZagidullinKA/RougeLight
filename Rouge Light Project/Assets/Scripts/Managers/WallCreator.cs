using UnityEngine;
using log4net;

public class WallCreator : MonoBehaviour
{
    [SerializeField] private float width;
    [SerializeField] private float height;
    [SerializeField] private Color wallColor = Color.red;
    [SerializeField] private bool hasCollider = true; //Нужен ли коллайдер?
    [SerializeField] private bool showCollider = true;
    [SerializeField] private bool isKinematic = true;
    [SerializeField] private float pixelsPerUnit;

    private static readonly ILog log = LogManager.GetLogger(typeof(Observer));


    void Start()
    {
        WallCreator wallCreator = GetComponent<WallCreator>();
        wallCreator.CreateWall(new Vector2(5, 3));
    }

    public void CreateWall(Vector2 position, Transform parent = null)
    {
        GameObject wall = new GameObject("Wall");
        wall.tag = "Wall";
        wall.layer = 11;
        wall.transform.position = position;

        if (parent != null)
            wall.transform.SetParent(parent);

        log.Debug("Pixels per unit is " + pixelsPerUnit);

        // 1. Создаём спрайт
        Texture2D texture = new Texture2D((int)pixelsPerUnit, (int)pixelsPerUnit);
        
        log.Debug("Sprite width and height is " + texture.width + " " + texture.height);
        
        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                texture.SetPixel(x, y, wallColor); // Заливаем белым
        texture.Apply();
      
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), // Pivot в центре
            pixelsPerUnit
        );

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = wallColor;

        // 2. Масштабируем спрайт (теперь 1 юнит Unity = 100 пикселей)
        wall.transform.localScale = new Vector2(width * pixelsPerUnit, height * pixelsPerUnit);

        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 3. Настраиваем коллайдер (размер в юнитах Unity)
        if (hasCollider)
        {
            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1, 1); // Теперь размер коллайдера = размеру стены
        }

        if (showCollider) 
        {
            // 4. Настраиваем LineRenderer (рисуем границы коллайдера)
            LineRenderer lineRenderer = wall.AddComponent<LineRenderer>();
            lineRenderer.useWorldSpace = false;
            lineRenderer.positionCount = 5;
            lineRenderer.loop = true;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.green;
            lineRenderer.endColor = Color.green;

            // Углы коллайдера (учитываем его размер)
            float halfWidth = 0.5f;
            float halfHeight = 0.5f;

            Vector3[] corners = new Vector3[5]
            {
            new Vector3(-halfWidth, -halfHeight, 0), // Левый нижний
            new Vector3(-halfWidth, halfHeight, 0),  // Левый верхний
            new Vector3(halfWidth, halfHeight, 0),   // Правый верхний
            new Vector3(halfWidth, -halfHeight, 0),  // Правый нижний
            new Vector3(-halfWidth, -halfHeight, 0) // Замыкаем
            };

            lineRenderer.SetPositions(corners);
        }
    }
}
