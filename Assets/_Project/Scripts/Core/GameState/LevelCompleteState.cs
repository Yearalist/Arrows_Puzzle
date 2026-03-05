using UnityEngine;

public class LevelCompleteState : IGameState
{
    public void Enter()
    {
        Debug.Log("[LevelComplete] Congratulations! Level completed!");
        Debug.Log("[LevelComplete] Press SPACE for next level, M for menu.");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartLevel();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.Instance.GoToMenu();
        }
    }

    public void Exit()
    {
        Debug.Log("[LevelComplete] Moving on...");
    }
}