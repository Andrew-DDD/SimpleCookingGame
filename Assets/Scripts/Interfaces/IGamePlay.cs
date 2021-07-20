public interface IGamePlay
{
    void GameStateChanges(GameState newState, GameRuleBase newRule);
    void ScoreChanges(int newScore);
    void TrayFigureCountChanges(int newCount);
    void SpawnCustomer(Customer newCustomer);
}