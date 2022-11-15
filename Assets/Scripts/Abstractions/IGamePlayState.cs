public interface IGamePlayState
{
    void GameStateChanges(GameState newState, IGameRule newRule);
    void ScoreChanges(int newScore);
    void TrayFigureCountChanges(int newCount);
    void SpawnCustomer(Customer newCustomer);
}