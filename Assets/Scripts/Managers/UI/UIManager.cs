#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;


public class UIManager : MonoBehaviour, IGamePlayState, IInput
{

    #region Fields

    [Header("StartGameUI")]
    [SerializeField] private GameObject startGamePanel;
    [SerializeField] private Text timeToStartText;

    [Header("GameUI")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text gameTimerText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Button figureButton;
    [SerializeField] private Button okButton;

    [Header("GameOverUI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Text gameOverScoreText;

    [Header("Customers")]
    [SerializeField] private CustomerPanel customersPanel;
    [SerializeField] private RectTransform parentCustomersPanel;

    [Header("Ref")]
    [SerializeField] private FigureData figureData = default;

    [Header("Parameters")]
    [SerializeField] private float CustomersPanelOffset = 175.0f;
    [SerializeField] private float FigureIconSize = 25.0f;

    private Button[] figureButtons;
    private Sprite[] figureIcons;
    private StringBuilder timerBuilderText = new StringBuilder(16);
    private StringBuilder scoreBuilderText = new StringBuilder(5);

    private IGameRule currentGameRule;
    private GameState currentGameState;

    #endregion


    #region IInput

    public event Action<int> AddFigureEvent;
    public event Action OrderEvent;
    public event Action RestartGameEvent;

    #endregion


    #region Initiolize
    private void Start()
    {
        figureIcons = figureData.GetFigureSprites();
        InitListeners();
    }

    private void InitListeners()
    {
        figureButtons = CreateFigureButtons();
        for (int i = 0; i < figureButtons.Length; i++)
        {
            int figure = i;
            figureButtons[i].onClick.AddListener(() => 
            { 
                OnAddFigure(figure); 
            });
        }
        restartButton.onClick.AddListener(OnRestartGame);
        okButton.onClick.AddListener(OnOrder);
    }

    private void Update()
    {
        if (currentGameRule == null)
        {
            return;
        }

        UpdateCurrentGameRule();
    }

    #endregion



    #region Panel States
    private void SetActiveState(GameState newState, IGameRule newRule)
    {
        DisableAllPanels();

        currentGameState = newState;
        currentGameRule = newRule;

        switch (newState)
        {
            case GameState.GameStart:
                startGamePanel.SetActive(true);
                break;
            case GameState.Game:
                gamePanel.SetActive(true);
                break;
            case GameState.GameOver:
                gameOverPanel.SetActive(true);
                UpdateGameOverScoreText();
                break;
        }
    }

    private void DisableAllPanels()
    {
        if (startGamePanel.activeSelf)
        {
            startGamePanel.SetActive(false);
        }

        if (gamePanel.activeSelf)
        {
            gamePanel.SetActive(false);
        }

        if (gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(false);
        }
    }

    #endregion



    #region Customers
    private CustomerPanel CreateCustomerPanel(Customer customer)
    {
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(customer.ConveyorPosition);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * parentCustomersPanel.sizeDelta.x) - (parentCustomersPanel.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * parentCustomersPanel.sizeDelta.y) - (parentCustomersPanel.sizeDelta.y * 0.5f)));

        CustomerPanel customer_panel = Instantiate(customersPanel);
        customer_panel.transform.SetParent(parentCustomersPanel.transform);
        customer_panel.transform.localScale = Vector3.one;
        customer_panel.gameObject.SetActive(true);
        Vector2 pos = WorldObject_ScreenPosition + Vector2.up * CustomersPanelOffset;
        customer_panel.Show(pos, customer, figureIcons);
        return customer_panel;
    }

    private void RemoveCustomerPanel(CustomerPanel panel)
    {
        panel.Hide();
    }

    #endregion



    #region FigureButtons

    private Button[] CreateFigureButtons()
    {
        Button[] figureButtons = new Button[figureIcons.Length];
        for (int i = 0; i < figureButtons.Length; i++)
        {
            figureButtons[i] = Instantiate(figureButton);
            figureButtons[i].transform.SetParent(figureButton.transform.parent);
            figureButtons[i].transform.SetSiblingIndex(0);
            figureButtons[i].transform.localScale = Vector3.one;
            figureButtons[i].gameObject.SetActive(true);
            Image iconImage = figureButtons[i].transform.Find("Icon").GetComponent<Image>();
            iconImage.sprite = figureIcons[i];
            iconImage.rectTransform.sizeDelta = new Vector2(figureIcons[i].rect.width / figureIcons[i].rect.height * FigureIconSize, FigureIconSize);
        }
        figureButton.gameObject.SetActive(false);
        return figureButtons;
    }

    private void UpdateFigureButtons(int count)
    {
        foreach (var button in figureButtons)
        {
            button.interactable = count >= figureButtons.Length ? false : true;
        }

        okButton.interactable = count > 0 ? true : false;
    }

    #endregion



    #region UpdateGameText

    private void UpdateCurrentGameRule() 
    {
        switch (currentGameState)
        {
            case GameState.GameStart:
                timerBuilderText.Clear();
                timerBuilderText.Append(currentGameRule.ProgressInfo());
                UpdateStartTimerText(timerBuilderText.ToString());
                break;
            case GameState.Game:
                timerBuilderText.Clear();
                timerBuilderText.Append(currentGameRule.Description());
                timerBuilderText.Append(currentGameRule.ProgressInfo());
                UpdateGameTimeText(timerBuilderText.ToString());
                break;
            default:
                break;
        }
    }

    private void UpdateStartTimerText(string value)
    {
        timeToStartText.text = value;
    }

    private void UpdateGameTimeText(string value)
    {
        gameTimerText.text = value;
    }

    private void UpdateScoreText(int score)
    {
        scoreBuilderText.Clear();
        scoreBuilderText.Append("SCORE: ");
        scoreBuilderText.Append(score.ToString());
        scoreText.text = scoreBuilderText.ToString();
    }

    private void UpdateGameOverScoreText()
    {
        gameOverScoreText.text = scoreText.text;
    }

    #endregion



    #region IGamePlay

    public void GameStateChanges(GameState newState, IGameRule newRule)
    {
        SetActiveState(newState, newRule);
    }

    public void ScoreChanges(int newScore)
    {
        UpdateScoreText(newScore);
    }

    public void TrayFigureCountChanges(int newCount)
    {
        UpdateFigureButtons(newCount);
    }

    public void SpawnCustomer(Customer newCustomer)
    {
        CustomerPanel customer_panel = CreateCustomerPanel(newCustomer);

        Action handler = null;
        handler = () =>
        {
            newCustomer.CustomerLeftEvent -= handler;
            RemoveCustomerPanel(customer_panel);
        };
        newCustomer.CustomerLeftEvent += handler;
    }

    #endregion



    #region IInput

    public void OnAddFigure(int figureIndex)
    {
        AddFigureEvent?.Invoke(figureIndex);
    }

    public void OnOrder()
    {
        OrderEvent?.Invoke();
    }

    public void OnRestartGame()
    {
        RestartGameEvent?.Invoke();
    }

    #endregion
}
