using UnityEngine;

public class PlayingState : IGameState
{
    public void Enter()
    {
        Debug.Log("[PlayingState] Game is now playing! Click arrows to solve the puzzle.");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.PauseGame();
        }
    }

    public void Exit()
    {
        Debug.Log("[PlayingState] Leaving play mode...");
    }
}