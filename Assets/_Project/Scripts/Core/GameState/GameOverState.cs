using UnityEngine;

public class GameOverState : IGameState
{
    public void Enter()
    {
        Debug.Log("[GameOver] Game Over! You ran out of lives.");
        Debug.Log("[GameOver] Press R to retry, M for menu.");
    }

    public void Update()
    {
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
        Debug.Log("[GameOver] Restarting...");
    }
}