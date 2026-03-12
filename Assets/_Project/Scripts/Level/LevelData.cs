using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ArrowPartData
{
    public int x;
    public int y;
}

[System.Serializable]
public struct ArrowData
{
    public int headX;
    public int headY;
    public ArrowDirection direction;
    public ArrowPartData[] bodyParts;
}

[CreateAssetMenu(fileName = "Level_00", menuName = "Arrows Puzzle/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Grid Settings")]
    public int gridWidth = 6;
    public int gridHeight = 6;

    [Header("Arrow Placements")]
    public ArrowData[] arrows;

    [Header("Level Info")]
    public int levelNumber;
    public int threeStarMoves;
    public int twoStarMoves;
}