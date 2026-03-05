using UnityEngine;

public class GameStateMachine
{
    private IGameState currentState;

    public IGameState CurrentState => currentState;

    public void ChangeState(IGameState newState)
    {
        if (currentState != null)
        {
            Debug.Log($"[StateMachine] Exiting: {currentState.GetType().Name}");
            currentState.Exit();
        }

        currentState = newState;
        Debug.Log($"[StateMachine] Entering: {currentState.GetType().Name}");
        currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
    }
}