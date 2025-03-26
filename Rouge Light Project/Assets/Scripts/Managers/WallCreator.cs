using UnityEngine;
using log4net;

public class WallCreator : MonoBehaviour
{
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    [SerializeField] private Color _wallColor = Color.red;
    [SerializeField] private bool hasCollider = true;
    [SerializeField] private bool showCollider = true;

    private static readonly ILog log = LogManager.GetLogger(typeof(Observer));

    // Публичные свойства с геттерами и сеттерами
    public float Width
    {
        get => _width;
        set => _width = value;
    }

    public float Height
    {
        get => _height;
        set => _height = value;
    }

    public Color WallColor
    {
        get => _wallColor;
        set => _wallColor = value;
    }

    public void CreateWall(Vector2 position, float angle = 0f, Transform parent = null)
    {
        GameObject wall = new GameObject("Wall");
        wall.tag = "Wall";
        wall.layer = 11;
        wall.transform.position = position;
        wall.transform.rotation = Quaternion.Euler(0f, 0f, angle);

        if (parent != null)
            wall.transform.SetParent(parent);

        // 1. Создаём спрайт
        Texture2D texture = new Texture2D(1, 1);

        for (int x = 0; x < texture.width; x++)
            for (int y = 0; y < texture.height; y++)
                texture.SetPixel(x, y, _wallColor);
        texture.Apply();

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            1
        );

        SpriteRenderer spriteRenderer = wall.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.color = _wallColor;

        // 2. Масштабируем спрайт
        wall.transform.localScale = new Vector2(_width, _height);

        Rigidbody2D rb = wall.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        // 3. Настраиваем коллайдер
        if (hasCollider)
        {
            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1, 1);
        }
    }
}