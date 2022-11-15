#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class CustomerPanel : MonoBehaviour
{

    #region Fields

    [Header("Parameters")]
    [SerializeField] private float figureIconSize = 25.0f;

    [Header("Properties")]
    [SerializeField] private Slider timeSlider;
    [SerializeField] private RectTransform rect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image[] iconImages;

    private Customer customer;
    private Tween fade;

    #endregion





    public void Show(Vector2 pos, Customer clt, Sprite[] figureIcs)
    {
        rect.anchoredPosition = pos;
        customer = clt;
        int[] figures = customer.GetFigures();
        for (int i = 0; i < figures.Length; i++)
        {
            iconImages[i].transform.parent.gameObject.SetActive(figures[i] != 0);
            if (figures[i] != 0)
            {
                iconImages[i].sprite = figureIcs[figures[i] - 1];
                iconImages[i].rectTransform.sizeDelta = new Vector2(iconImages[i].sprite.rect.width / iconImages[i].sprite.rect.height * figureIconSize, figureIconSize);
            }
        }
        canvasGroup.alpha = 0.0f;
        fade = canvasGroup.DOFade(1.0f, 0.25f).SetDelay(0.5f);
    }

    public void Hide()
    {
        fade.Kill();
        fade = canvasGroup.DOFade(0.0f, 0.25f).OnComplete(()=> 
        {
            Destroy(this.gameObject);
        });
    }

    private void LateUpdate()
    {
        if (!customer) return;

        timeSlider.SetValueWithoutNotify(customer.Timer / customer.WaitingTime);
    }
}
