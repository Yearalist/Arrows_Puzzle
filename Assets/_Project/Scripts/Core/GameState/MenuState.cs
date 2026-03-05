using UnityEngine;

public class MenuState : IGameState
{
    public void Enter()
    {
        Debug.Log("[MenuState] Welcome to Arrows Puzzle Escape!");
        Debug.Log("[MenuState] Press SPACE to start playing.");
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameManager.Instance.StartLevel();
        }
    }

    public void Exit()
    {
        Debug.Log("[MenuState] Leaving menu...");
    }
}