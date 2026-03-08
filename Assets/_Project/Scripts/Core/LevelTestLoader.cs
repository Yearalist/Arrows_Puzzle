using UnityEngine;
using Cysharp.Threading.Tasks;

public class LevelTestLoader : MonoBehaviour
{
    [SerializeField] private LevelData testLevel;
    [SerializeField] private LevelLoader levelLoader;

    private void Start()
    {
        if (testLevel != null && levelLoader != null)
        {
            LoadTestLevel().Forget();
        }
        else
        {
            Debug.LogError("[LevelTestLoader] Assign testLevel and levelLoader in Inspector!");
        }
    }

    private async UniTaskVoid LoadTestLevel()
    {
        await UniTask.Delay(500);
        await levelLoader.LoadLevel(testLevel);
        Debug.Log("[LevelTestLoader] Test level loaded!");
    }
}