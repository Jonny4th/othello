using UnityEngine;

public class BoardCreator
{
    public static T[,] CreateBoard<T>(T prototype, int width, int height, Transform parent) where T : MonoBehaviour, ICell
    {
        var cells = new T[width, height];

        Vector3 size = prototype.GetComponent<SpriteRenderer>().bounds.size;

        float startPosX = size.x * width / 2f - 0.5f;
        float startPosY = size.y * height / 2f - 0.5f;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                var parameter = new InstantiateParameters()
                {
                    parent = parent,
                    worldSpace = false
                };

                var posX = x * size.x - startPosX;
                var posY = y * size.y - startPosY;
                var cell = Object.Instantiate(prototype, new Vector3(posX, posY, 0), Quaternion.identity, parameter);

                cell.SetCoordinates(x, y);
                cell.name = $"Cell_{x}_{y}";
                cells[x, y] = cell;
            }
        }

        return cells;
    }
}
