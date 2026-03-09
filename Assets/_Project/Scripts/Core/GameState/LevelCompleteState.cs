using UnityEngine;

public class LevelCompleteState : IGameState
{
    public void Enter()
    {
        Debug.Log("[LevelComplete] Congratulations! Level completed!");
        Debug.Log("[LevelComplete] Press SPACE for next level, R to retry, M for menu.");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LevelManager.Instance.LoadNextLevel();
            GameManager.Instance.StateMachine.ChangeState(new PlayingState());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelManager.Instance.RestartCurrentLevel();
            GameManager.Instance.StateMachine.ChangeState(new PlayingState());
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