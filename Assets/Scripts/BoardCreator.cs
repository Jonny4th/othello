using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    [SerializeField]
    private Cell m_CellPrototype;

    [SerializeField]
    private Transform m_CellParent;

    [SerializeField]
    private int m_Width = 8;

    [SerializeField]
    private int m_Height = 8;

    private void Start()
    {
        CreateBoard();
    }

    public Dictionary<(int,int),Cell> CreateBoard()
    {
        var cells = new Dictionary<(int, int), Cell>();

        Vector3 size = m_CellPrototype.GetComponent<SpriteRenderer>().bounds.size;

        float startPosX = size.x * m_Width / 2f - 0.5f;
        float startPosY = size.y * m_Height / 2f - 0.5f;

        for (int x = 0; x < m_Width; x++)
        {
            for (int y = 0; y < m_Height; y++)
            {
                var posX = x * size.x - startPosX;
                var posY = y * size.y - startPosY;
                Cell cell = Instantiate(m_CellPrototype, new Vector3(posX, posY, 0), Quaternion.identity, m_CellParent);

                cell.SetCoordinates(x, y);
                cell.name = $"Cell_{x}_{y}";
                cells.Add((x, y), cell);
            }
        }

        return cells;
    }
}
