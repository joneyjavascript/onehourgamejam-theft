using UnityEngine.UI;

[System.Serializable]
public class Cell
{
    public int x;
    public int y;
    public Image image;

    public Cell(int cellX, int cellY, Image image)
    {
        this.x = cellX;
        this.y = cellY;
        this.image = image;
    }
}

