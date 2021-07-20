#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class UIManager : MonoBehaviour, IGamePlay
{

    //--------------------------------------------------------------
    //Properties
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
    [SerializeField, InterfaceType(typeof(IInput))] private Object gameInputInterfaceObj = default;
    private IInput gameInputInterface => gameInputInterfaceObj as IInput;
    //~Properties
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Parameters
    [Header("Parameters")]
    [SerializeField] private float CustomersPanelOffset = 175.0f;
    [SerializeField] private float FigureIconSize = 25.0f;
    //~Parameters
    //--------------------------------------------------------------


    //--------------------------------------------------------------
    //Variables
    private Button[] figureButtons;
    private Sprite[] figureIcons;
    private string currentGameRuleDesc;
    //~Variables
    //--------------------------------------------------------------



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
            figureButtons[i].onClick.AddListener(() => { gameInputInterface.AddFigure(figure); });
        }
        restartButton.onClick.AddListener(gameInputInterface.RestartGame);
        okButton.onClick.AddListener(gameInputInterface.Order);
    }
    #endregion



    #region Panel States
    private void SetActiveState(GameState newState, GameRuleBase newRule)
    {
        DisableAllPanels();

        switch (newState)
        {
            case GameState.GameStart:
                startGamePanel.SetActive(true);
                newRule.m_UpdateGameRuleEvent.AddListener(UpdateStartTimerText);
                break;
            case GameState.Game:
                gamePanel.SetActive(true);
                newRule.m_UpdateGameRuleEvent.AddListener(UpdateGameTimeText);
                currentGameRuleDesc = newRule.Description;
                UpdateGameTimeText(newRule.ProgressInfo);
                break;
            case GameState.GameOver:
                gameOverPanel.SetActive(true);
                UpdateGameOverScoreText();
                break;
        }
    }
    private void DisableAllPanels()
    {
        if (startGamePanel.activeSelf) startGamePanel.SetActive(false);
        if (gamePanel.activeSelf) gamePanel.SetActive(false);
        if (gameOverPanel.activeSelf) gameOverPanel.SetActive(false);
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
            button.interactable = count >= figureButtons.Length ? false : true;
        okButton.interactable = count > 0 ? true : false;
    }
    #endregion



    #region UpdateGameText
    private void UpdateStartTimerText(string value)
    {
        timeToStartText.text = value;
        timeToStartText.transform.localScale = Vector3.zero;
        timeToStartText.transform.DOScale(1.0f, 0.1f);
    }
    private void UpdateGameTimeText(string value)
    {
        gameTimerText.text = currentGameRuleDesc + value;
    }
    private void UpdateScoreText(int score)
    {
        scoreText.text = "SCORE: " + score.ToString();
    }
    private void UpdateGameOverScoreText()
    {
        gameOverScoreText.text = scoreText.text;
    }
    #endregion



    #region IGamePlay
    public void GameStateChanges(GameState newState, GameRuleBase newRule)
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
        newCustomer.m_CustomerLeftEvent.AddListener(() => { RemoveCustomerPanel(customer_panel); });
    }
    #endregion
}
