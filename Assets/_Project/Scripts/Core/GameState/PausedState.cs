using UnityEngine;

public class PausedState : IGameState
{
    public void Enter()
    {
        Debug.Log("[PausedState] Game paused. Press SPACE to resume, M for menu.");
        Time.timeScale = 0f;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.ResumeGame();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.Instance.GoToMenu();
        }
    }

    public void Exit()
    {
        Time.timeScale = 1f;
        Debug.Log("[PausedState] Resuming...");
    }
}