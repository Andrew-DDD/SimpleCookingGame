using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    #region Fields

    [Header("Parameters")]
    [SerializeField] private float SpawnCustomersDelay = 3.0f;
    [SerializeField] private int TimeToStart = 3;
    [SerializeField] private int GameTimer = 15;
    [SerializeField] [MinMax(1, 20, ShowEditRange = true)] private Vector2 CustomerWaitingTime = new Vector2(5, 15);

    [SerializeField] private FigureData figureData = default;
    [SerializeField] private ConveyorManager conveyorManager = default;

    [SerializeField, InterfaceType(typeof(IGamePlayState))] private UnityEngine.Object[] gamePlayObjs = default;
    private List<IGamePlayState> gamePlayInterfaces;

    [SerializeField, InterfaceType(typeof(IInput))] private UnityEngine.Object[] gameInputInterfaceObj = default;
    private List<IInput> gameInputInterface;

    private GameState currentGameState;
    private IGameRule currentGameRule;

    private ScoreManager scoreManager = default;

    #endregion


    #region Properties

    public GameState GetGameState() => currentGameState;
    public IGameRule GetCurrentGameRule() => currentGameRule;

    public ConveyorManager GetConveyorManager() => conveyorManager;
    public FigureData GetFigureData() => figureData;

    public bool IsGame => currentGameState == GameState.Game;

    #endregion



    #region Initiolize
    
    private void OnEnable()
    {
        Initiolize();
    }

    private void OnDisable()
    {
        gameInputInterface.ForEach(i => i.AddFigureEvent -= AddFigure);
        gameInputInterface.ForEach(i => i.OrderEvent -= Order);
        gameInputInterface.ForEach(i => i.RestartGameEvent -= RestartGame);

        conveyorManager.SpawnCustomerEvent -= SpawnCustomer;
        conveyorManager.TrayMoveToTrashEvent -= TrayMoveToTrash;
        conveyorManager.TrayFigureCountEvent -= TrayFigureCountChanges;
    }

    private void Initiolize()
    {
        gamePlayInterfaces = gamePlayObjs.Select(o => o as IGamePlayState).ToList();
        gameInputInterface = gameInputInterfaceObj.Select(o => o as IInput).ToList();

        gameInputInterface.ForEach(i => i.AddFigureEvent += AddFigure);
        gameInputInterface.ForEach(i => i.OrderEvent += Order);
        gameInputInterface.ForEach(i => i.RestartGameEvent += RestartGame);

        conveyorManager.SpawnCustomerEvent += SpawnCustomer;
        conveyorManager.TrayMoveToTrashEvent += TrayMoveToTrash;
        conveyorManager.TrayFigureCountEvent += TrayFigureCountChanges;

        if (!scoreManager)
        {
            scoreManager = gameObject.CreateComponent<ScoreManager>("ScoreManager") as ScoreManager;
        }

        scoreManager.ResetScore();
        ScoreChanges(scoreManager.Score);

        SetGameState(GameState.GameStart);
    }

    private void Update()
    {
        if (currentGameRule == null)
        {
            return;
        }

        currentGameRule.Tick(Time.deltaTime);
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

    private IGameRule GameStart()
    {
        IGameRule startTimer = new GameRuleTimer(TimeToStart, true, false);
        startTimer.Init();

        Action handler = null;
        handler = () =>
        {
            SetGameState(GameState.Game);
            startTimer.GameStoppedEvent -= handler;
        };
        startTimer.GameStoppedEvent += handler;

        return startTimer;
    }

    private IGameRule Game()
    {
        //Here you can make other game rules
        IGameRule gameTimer = new GameRuleTimer(GameTimer, true, true);
        gameTimer.Init();

        Action handler = null;
        handler = () =>
        {
            SetGameState(GameState.GameOver);
            gameTimer.GameStoppedEvent -= handler;
        };
        gameTimer.GameStoppedEvent += handler;

        //Initiolize conveyor
        conveyorManager.StartConveyor(SpawnCustomersDelay, CustomerWaitingTime, figureData.GetFigureObjects());
        return gameTimer;
    }

    private IGameRule GameOver()
    {
        conveyorManager.StopConveyor();
        return null;
    }

    #endregion




    #region Actions

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

        if (conveyorManager.IsConveyorWorking)
        {
            conveyorManager.StopConveyor();
        }

        currentGameRule = null;

        SetGameState(GameState.GameStart);
    }

    #endregion




    #region GamePlay

    private void GameStateChanges(GameState newState, IGameRule newRule)
    {
        gamePlayInterfaces.ForEach(i => i.GameStateChanges(newState, newRule));
    }

    private void ScoreChanges(int newScore)
    {
        gamePlayInterfaces.ForEach(i => i.ScoreChanges(newScore));
    }

    private void TrayFigureCountChanges(int newCount)
    {
        gamePlayInterfaces.ForEach(i => i.TrayFigureCountChanges(newCount));
    }

    private void SpawnCustomer(Customer newCustomer)
    {
        if (!IsGame)
        {
            return;
        }

        Action<bool> handler = null;
        handler = (arg) =>
        {
            newCustomer.CustomerEvent -= handler;
            CustomerLeft(arg);
        };
        newCustomer.CustomerEvent += handler;

        gamePlayInterfaces.ForEach(i => i.SpawnCustomer(newCustomer));
    }

    private void CustomerLeft(bool timeOut) 
    {
        if (timeOut)
        {
            scoreManager.MinusScore();
        }
        else
        {
            scoreManager.AddScore();
        }

        ScoreChanges(scoreManager.Score);
    }

    private void TrayMoveToTrash() 
    {
        if (!IsGame)
        {
            return;
        }
        scoreManager.MinusScore();
        ScoreChanges(scoreManager.Score);
    }

    #endregion
}
