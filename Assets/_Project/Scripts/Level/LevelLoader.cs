using UnityEngine;
using Cysharp.Threading.Tasks;
public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GridSystem gridSystem;


    public async UniTask LoadLevel(LevelData levelData)
    {
        Debug.Log($"[LevelLoader] Loading Level {levelData.levelNumber}...");

        DestroyOldArrows();
        gridSystem.ClearGrid();

        await UniTask.Delay(100);

        gridSystem.CreateGrid(levelData.gridWidth, levelData.gridHeight);

        await UniTask.Delay(100);

        PlaceArrows(levelData);

        EventBus.Publish(new LevelStartedEvent { levelNumber = levelData.levelNumber });

        Debug.Log($"[LevelLoader] Level {levelData.levelNumber} loaded successfully!");
    }



    private void DestroyOldArrows()
    {
        Arrow[] oldArrows = FindObjectsOfType<Arrow>(true);

        foreach (Arrow arrow in oldArrows)
        {
            Destroy(arrow.gameObject);
        }

        Debug.Log($"[LevelLoader] Destroyed {oldArrows.Length} old arrows");
    }


    [SerializeField] private ArrowFactory arrowFactory;

    private void PlaceArrows(LevelData levelData)
    {
        float offsetX = (levelData.gridWidth - 1) * 1.2f * 0.5f;
        float offsetY = (levelData.gridHeight - 1) * 1.2f * 0.5f;
        Vector2 gridOffset = new Vector2(offsetX, offsetY);

        foreach (ArrowData arrowData in levelData.arrows)
        {
            Cell headCell = gridSystem.GetCell(arrowData.headX, arrowData.headY);
            if (headCell != null)
            {
                headCell.SetOccupied(true);
            }

            if (arrowData.bodyParts != null)
            {
                foreach (ArrowPartData part in arrowData.bodyParts)
                {
                    Cell bodyCell = gridSystem.GetCell(part.x, part.y);
                    if (bodyCell != null)
                    {
                        bodyCell.SetOccupied(true);
                    }
                }
            }

            arrowFactory.CreateArrow(arrowData, gridOffset);
        }

        gridSystem.SetActiveArrowCount(levelData.arrows.Length);
    }
}




