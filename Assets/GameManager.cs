using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SocialSlot {
    public Sprite sprite;
}

public class GameManager : MonoBehaviour {

    public Sprite[] socialSpritesPossibles;
    public Sprite matchSprite;
    public SocialSlot[][] socialSlots;
    public static GameManager managerInstance;
    public float timer = 0;
    public Text scoreText;
    private int _score;
    public int score { get { return PlayerPrefs.GetInt("score"); } set { PlayerPrefs.SetInt("score", value); scoreText.text = "Hacked Data From " + value.ToString() + " Users. ";  } }
       
    private void Start()
    {
        if(managerInstance == null) managerInstance = this;
        
        resetGame();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0) {
            resetGame();
        }
    }

    void resetGame() {
        timer = 5;
        CreateMatriz();
        RandomAll();
        UpdateView();        
    }

    void SetSprite(Sprite sprite, int cellX, int cellY) {
        socialSlots[cellX][cellY].sprite = sprite;
    }

    void RandomAll()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Sprite sprite = GetRandomSocialSprite();
                SetSprite(sprite, i, j);
            }
        }
    }

    public List<Cell> GetListOfEqualsCells(int cellX, int cellY) {
        // Initialize lists to control process
        List<Cell> cellsEquals = new List<Cell>();
        List<Cell> cellsAroundOneCell = new List<Cell>();
        List<Cell> cellsToAdd = new List<Cell>();
        List<Cell> cellsAlreadyAdded = new List<Cell>();

        // Cells that are equals
        cellsEquals.Clear();
        cellsEquals = GetCellsAround(cellX, cellY, cellsEquals);

        // Cells addeds
        cellsAlreadyAdded.Clear();
        cellsAlreadyAdded.AddRange(cellsEquals);

        // Process to add all equals cells around
        int infinityLoopPrevent = 0;
        do
        {
            cellsToAdd.Clear();
            foreach (Cell cellItem in cellsEquals)
            {
                cellsAroundOneCell.Clear();
                cellsAroundOneCell = GetCellsAround(cellItem.x, cellItem.y, cellsAlreadyAdded);

                cellsToAdd.AddRange(cellsAroundOneCell);
                cellsAlreadyAdded.AddRange(cellsAroundOneCell);
            }

            cellsEquals.AddRange(cellsToAdd);

            // Prevent infinity loop here
            if ((infinityLoopPrevent++) > 100)
            {
                Debug.LogError("Infinity loop inside 'while' statement");
                break;
            }

        } while (cellsToAdd.Count > 0);

        return cellsEquals;
    }


    public void OnClickInSocialCell(int cellX, int cellY) {

        Cell cell = GetCell(cellX, cellY);

        if (!IsSocialPossibleSprite(cell.image.sprite)) {
            Debug.Log("Invalid Cell clicked to Match");
            return;
        }

        List <Cell> cellsEqualToTheClickedCell  = GetListOfEqualsCells(cellX, cellY);
        MatchCellsIfIsPossible(cellsEqualToTheClickedCell);
    }

    void MatchCellsLeftOver() {
        if (!HasPossibleMatchCellsInGrid())
        {
            List<Cell> cellsLeftOver = GetCellsLeftOver();
            MatchCells(cellsLeftOver);
        }
    }

    public List<Cell> GetCellsLeftOver() {
        List<Cell> cellsLeftOver = new List<Cell>();
        foreach (Cell cell in GetAllCells()) {
            if (IsSocialPossibleSprite(cell.image.sprite)) {
                cellsLeftOver.Add(cell);
            }
        }
        return cellsLeftOver;
    }

    public bool IsSocialPossibleSprite(Sprite searchSprite)
    {          
        foreach (Sprite sprite in socialSpritesPossibles)
        {
            if (sprite == searchSprite)
            {
                return true;
            }
        }
        return false;
    }


    public List<Cell> GetAllCells()
    {
        List<Cell> allCells = new List<Cell>();

        for (int cellX = 0; cellX < 5; cellX++)
        {
            for (int cellY = 0; cellY < 5; cellY++)
            {
                Image image = GetImage(cellX, cellY);
                Cell cell = new Cell(cellX, cellY, image);

                allCells.Add(cell);
            }
        }

        return allCells;
    }

    public bool HasPossibleMatchCellsInGrid() {
        for (int cellX = 0; cellX < 5; cellX++)
        {
            for (int cellY = 0; cellY < 5; cellY++)
            {                
                Image image = GetImage(cellX, cellY);
                Cell cell = new Cell(cellX, cellY, image);

                bool possibleSocialSprite = false;
                foreach (Sprite sprite in socialSpritesPossibles) {
                    if (sprite == cell.image.sprite) {
                        possibleSocialSprite = true;
                        break;
                    }
                }

                if (possibleSocialSprite)
                {
                    List<Cell> equals = GetListOfEqualsCells(cellX, cellY);
                    if (IsPossibleToMatch(equals))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public bool IsPossibleToMatch(List<Cell> listOfCells, int minMatchCount = 3) {
        return (listOfCells.Count >= minMatchCount);
    }

    public void MatchCellsIfIsPossible(List<Cell> CellsToMatch)
    {
        if (IsPossibleToMatch(CellsToMatch))
        {
            MatchCells(CellsToMatch);
        }
        else
        {
            Debug.Log("No Match. " + CellsToMatch.Count);
        }
    }

    public void MatchCells(List<Cell> CellsToMatch)
    {      
        Debug.Log("Match " + CellsToMatch + " equals cell");
        foreach (Cell cell in CellsToMatch)
        {
            MatchOneCell(cell);
        }       
    }

    public void MatchOneCell(Cell cell) {
        IEnumerator matchAnimation = AnimatedMatch(cell);
        StartCoroutine(matchAnimation);
        score += (int)timer;
    }

    public IEnumerator<bool> AnimatedMatch(Cell cell, float animationTime = .25f) {

        float alpha = 1;
        cell.image.sprite = GetMatchSprite();

        while (alpha > 0) {
            alpha -= Time.deltaTime / animationTime;
            Color newColor = cell.image.color;
            newColor.a = alpha;
            cell.image.color = newColor;
            yield return false;
        }
        
        cell.image.sprite = GetMatchSprite();

        while (alpha < 1)
        {
            alpha += Time.deltaTime / animationTime;
            Color newColor = cell.image.color;
            newColor.a = alpha;
            cell.image.color = newColor;
            yield return false;
        }

        yield return true;
    }

    Cell GetCell(int cellX, int cellY) {
        // clicked cell
        Image clickedSocialImage = GetImage(cellX, cellY);
        Cell clickedCell = new Cell(cellX, cellY, clickedSocialImage);

        return clickedCell;
    }


    List<Cell> GetCellsAround(int cellX, int cellY, List<Cell> ignoredCells) {

        List<Cell> equals = new List<Cell>();

        // clicked cell
        Cell clickedCell = GetCell(cellX, cellY);
        Image clickedSocialImage = clickedCell.image;

        if (!CellExistInList(ignoredCells, clickedCell))
        {
            equals.Add(clickedCell);
        }

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if ((Mathf.Abs(i) + Mathf.Abs(j)) == 2) continue; // disregard diagonal

                int closeCellX = cellX + i;
                int closeCellY = cellY + j;
                Image closerImage = GetImage(closeCellX, closeCellY);
                Cell closerCell = new Cell(closeCellX, closeCellY, closerImage);
                
                if (closerImage == null) continue; // outside of grid
                if (closerImage.sprite != clickedSocialImage.sprite) continue; // social icons not be different
                if (CellExistInList(equals, closerCell)) continue; // cell is already in the list
                if (CellExistInList(ignoredCells, closerCell)) continue; // cell is in ignored cell list
                
                equals.Add(closerCell);
            }
        }

        return equals;
    }

    bool CellExistInList(List<Cell> cellList, Cell item) {
        bool existsInList = false;
        foreach (Cell cell in cellList)
        {
            if (cell.x == item.x && cell.y == item.y)
            {
                existsInList = true;
                break;
            }
        }

        return existsInList;
    }

    Sprite GetMatchSprite() {
        return matchSprite;
    }

    Image GetImage(int cellX, int cellY) {

        if (GetSocialGameObjectFrom(cellX, cellY) == null) {
            return null;
        }

        return GetSocialGameObjectFrom(cellX, cellY).GetComponent<Image>();
    }

    void UpdateView() {
        for (int i = 0; i < 5; i++)
        {           
            for (int j = 0; j < 5; j++)
            {
                SocialSlot slot = socialSlots[i][j];

                GameObject uiImage = GetSocialGameObjectFrom(i, j);
                if (uiImage)
                {
                    Image image = uiImage.GetComponent<Image>();
                    image.sprite = slot.sprite;
                }
                else {
                    Debug.LogError("image not found");
                }
            }
        }
    }

    GameObject GetSocialGameObjectFrom(int cellX, int cellY) {
        return GameObject.Find("social_" + cellX + "_" + cellY);
    }

    Sprite GetRandomSocialSprite() {
        return socialSpritesPossibles[Random.Range(0, socialSpritesPossibles.Length - 1)];
    }

    void CreateMatriz() {
        socialSlots = new SocialSlot[5][];
        for (int i = 0; i < 5; i++)
        {
            socialSlots[i] = new SocialSlot[5];
            for (int j = 0; j < 5; j++)
            {
                socialSlots[i][j] = new SocialSlot();
                GameObject uiImage = GetSocialGameObjectFrom(i, j);
                uiImage.AddComponent<SocialButton>();
            }
        }
    }

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

}
