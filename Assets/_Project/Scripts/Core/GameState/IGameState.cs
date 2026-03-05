// Interface that every game state must follow
// Same concept as IState in the woodcutter example
// Enter = state starts, Update = runs every frame, Exit = state ends

public interface IGameState
{
    void Enter();
    void Update();
    void Exit();
}