using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(WallCreator))]
public class WallSquareCreator : MonoBehaviour
{
    [Header("Square Settings")]
    [SerializeField] private float _squareSize;

    [Header("Wall Settings")]
    [SerializeField] private float _wallThickness;
    [SerializeField] private Color _wallColor = Color.red;

    private WallCreator _wallCreator;
    private GameObject _wallsContainer;

    public float SquareSize
    {
        get => _squareSize;
        set => _squareSize = Mathf.Max(0.1f, value);
    }

    public float WallThickness
    {
        get => _wallThickness;
        set => _wallThickness = Mathf.Max(0.05f, value);
    }

    void Awake()
    {
        _wallCreator = GetComponent<WallCreator>();
        CreateWallsContainer();
    }

    void Start()
    {
        GenerateSquare();
    }

    public void GenerateSquare()
    {
        CreateSquareWalls();
    }

    private void CreateWallsContainer()
    {
        if (_wallsContainer == null)
        {
            _wallsContainer = new GameObject("WallsContainer");
            _wallsContainer.transform.SetParent(transform);
            _wallsContainer.transform.localPosition = Vector3.zero;
        }
    }

    private void CreateSquareWalls()
    {
        Vector2 center = (Vector2)transform.position;

        // Нижняя стена
        CreateWall(
            position: center + new Vector2(0, -_squareSize / 2),
            width: _squareSize + 1,
            height: _wallThickness,
            angle: 0f
        );

        // Правая стена
        CreateWall(
            position: center + new Vector2(_squareSize / 2, 0),
            width: _squareSize + 1,
            height: _wallThickness,
            angle: 90f
        );

        // Верхняя стена
        CreateWall(
            position: center + new Vector2(0, _squareSize / 2),
            width: _squareSize + 1,
            height: _wallThickness,
            angle: 0f
        );

        // Левая стена
        CreateWall(
            position: center + new Vector2(-_squareSize / 2, 0),
            width: _squareSize + 1,
            height: _wallThickness,
            angle: 90f
        );
    }

    private void CreateWall(Vector2 position, float width, float height, float angle)
    {
        _wallCreator.Width = width;
        _wallCreator.Height = height;
        _wallCreator.WallColor = _wallColor;
        _wallCreator.CreateWall(position, angle, _wallsContainer.transform);
    }
}