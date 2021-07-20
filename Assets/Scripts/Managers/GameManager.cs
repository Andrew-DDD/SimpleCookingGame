using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour, IInput, IGamePlay
{

    //--------------------------------------------------------------
    //Game State
    private GameState currentGameState;
    private GameRuleBase currentGameRule;
    //~Game State
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Parameters
    [Header("Parameters")]
    [SerializeField] private float SpawnCustomersDelay = 3.0f;
    [SerializeField] private float TimeToStart = 3.0f;
    [SerializeField] private float GameTimer = 15.0f;
    [SerializeField] [MinMax(1, 20, ShowEditRange = true)] private Vector2 CustomerWaitingTime = new Vector2(5, 15);
    //~Parameters
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Ref
    [Header("Ref")]
    [SerializeField] private FigureData figureData = default;
    [SerializeField] private ConveyorManager conveyorManager = default;

    [SerializeField, InterfaceType(typeof(IGamePlay))] private Object[] gamePlayObjs = default;
    private List<IGamePlay> gamePlayInterfaces;

    private ScoreManager scoreManager = default;
    //~Ref
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Get
    public GameState GetGameState() { return currentGameState; }
    public GameRuleBase GetCurrentGameRule() { return currentGameRule; }

    public ConveyorManager GetConveyorManager() { return conveyorManager; }
    public FigureData GetFigureData() { return figureData; }

    public bool IsGame { get { return currentGameState == GameState.Game; } }
    //~Get
    //--------------------------------------------------------------



    #region Initiolize
    private void Start()
    {
        Initiolize();

        SetGameState(GameState.GameStart);
    }
    private void Initiolize()
    {
        gamePlayInterfaces = gamePlayObjs.Select(o => o as IGamePlay).ToList();

        if (!scoreManager) scoreManager = gameObject.CreateComponent<ScoreManager>("ScoreManager") as ScoreManager;
        conveyorManager.m_SpawnCustomerEvent.AddListener((customer) =>
        {
            if (!IsGame) return;
            customer.m_CustomerEvent.AddListener((time_out) =>
            {
                if (time_out) scoreManager.MinusScore();
                else scoreManager.AddScore();
                ScoreChanges(scoreManager.Score);
            });
        });
        conveyorManager.m_TrayMoveToTrashEvent.AddListener(() =>
        {
            if (!IsGame) return;
            scoreManager.MinusScore();
            ScoreChanges(scoreManager.Score);
        });

        conveyorManager.m_TrayFigureCountEvent.AddListener(TrayFigureCountChanges);
        conveyorManager.m_SpawnCustomerEvent.AddListener(SpawnCustomer);

        scoreManager.ResetScore();
        ScoreChanges(scoreManager.Score);
    }
    #endregion



    #region States
    private void SetGameState(GameState newState)
    {
        currentGameState = newState;

        switch (currentGameState)
        {
            case GameState.GameStart:
                currentGameRule = GameStart();
                break;
            case GameState.Game:
                currentGameRule = Game();
                break;
            case GameState.GameOver:
                currentGameRule = GameOver();
                break;
            default:
                break;
        }
        GameStateChanges(currentGameState, currentGameRule);
    }
    private GameRuleBase GameStart()
    {
        GameRuleTimer startTimer = gameObject.CreateComponent<GameRuleTimer>("StartTimer") as GameRuleTimer;
        startTimer.Init(new object[] { TimeToStart, true, false });
        startTimer.m_GameRuleStoppedEvent.AddListener(() => 
        {
            SetGameState(GameState.Game);
            GameObject.Destroy(startTimer.gameObject);
        });
        return startTimer;
    }
    private GameRuleBase Game()
    {
        //Here you can make other game rules
        GameRuleTimer gameTimer = gameObject.CreateComponent<GameRuleTimer>("GameTimer") as GameRuleTimer;
        gameTimer.Init(new object[] { GameTimer, true, true });
        gameTimer.m_GameRuleStoppedEvent.AddListener(() =>
        {
            SetGameState(GameState.GameOver);
            GameObject.Destroy(gameTimer.gameObject);
        });

        //Initiolize conveyor
        conveyorManager.StartConveyor(SpawnCustomersDelay, CustomerWaitingTime, figureData.GetFigureObjects());
        return gameTimer;
    }
    private GameRuleBase GameOver()
    {
        conveyorManager.StopConveyor();
        return null;
    }
    #endregion




    #region IInputInterface
    public void AddFigure(int figure)
    {
        conveyorManager.AddFigure(figure);
    }
    public void Order()
    {
        conveyorManager.EditTrayComplete();
    }
    public void RestartGame()
    {
        scoreManager.ResetScore();
        ScoreChanges(scoreManager.Score);

        if (conveyorManager.ConveyorIsWorking) conveyorManager.StopConveyor();
        if (currentGameRule) GameObject.Destroy(currentGameRule.gameObject);
        SetGameState(GameState.GameStart);
    }
    #endregion




    #region IGamePlay
    public void GameStateChanges(GameState newState, GameRuleBase newRule)
    {
        gamePlayInterfaces.ForEach(i => i.GameStateChanges(newState, newRule));
    }
    public void ScoreChanges(int newScore)
    {
        gamePlayInterfaces.ForEach(i => i.ScoreChanges(newScore));
    }
    public void TrayFigureCountChanges(int newCount)
    {
        gamePlayInterfaces.ForEach(i => i.TrayFigureCountChanges(newCount));
    }
    public void SpawnCustomer(Customer newCustomer)
    {
        gamePlayInterfaces.ForEach(i => i.SpawnCustomer(newCustomer));
    }
    #endregion
}
