using UnityEngine;

[System.Serializable]
public struct ArrowData
{
    public int x;
    public int y;
    public ArrowDirection direction;
}


[CreateAssetMenu(fileName = "Level_00", menuName = "Arrows Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Grid Settings")]
    public int gridWidth = 4;
    public int gridHeight = 4;

    [Header("Arrow Placements")]
    public ArrowData[] arrows;

    [Header("Level Info")]
    public int levelNumber;
    public int threeStarMoves;
    public int twoStarMoves;
}
