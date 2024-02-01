using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using DG.Tweening;
using System;

public class Window : MonoBehaviour
{

    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _minimizeButton;
    [SerializeField] private MMF_Player _closingFeedbacks;
    [SerializeField] private MMF_Player _minimizingFeedbacks;

    [SerializeField] private RawImage _backgroundRawImage;

    private void Start()
    {
        _closeButton.onClick.AddListener(() => 
        {
            _closeButton.interactable = false;
            Close();
        });
        _minimizeButton.onClick.AddListener(() => 
        {
            Minimize();          
        });
    }

    private void OnDestroy()
    {
        _closeButton.onClick.RemoveAllListeners();
        _minimizeButton.onClick.RemoveAllListeners();
    }

    private void OnEnable()
    {
        _closeButton.interactable = true;
    }

    public void Open()
    {
        transform.SetAsLastSibling();
        //_closeButton.interactable = true;
        _minimizingFeedbacks.Direction = MMFeedbacks.Directions.BottomToTop;
        _minimizingFeedbacks.PlayFeedbacks();
    }

    private void Close()
    {
        _closingFeedbacks.PlayFeedbacks();
        ComputerControllerUI.Instance.CloseWindowEffects();
        DOVirtual.DelayedCall(_closingFeedbacks.TotalDuration + 0.1f, () => ComputerControllerUI.Instance.CloseWindow(this));
    }

    public void Minimize()
    {
        ComputerControllerUI.Instance.SetIsDesktopState();
        ComputerControllerUI.Instance.MinimizeWindowEffects();
        _minimizingFeedbacks.Direction = MMFeedbacks.Directions.TopToBottom;
        _minimizingFeedbacks.PlayFeedbacks();
    }

    public void ToDefault()
    {
        gameObject.SetActive(false);
        GetComponent<CanvasGroup>().alpha = 1f;
    }

    public void SetMinigameRenderTexture(RenderTexture renderTexture)
    {
        _backgroundRawImage.texture = renderTexture;
    }
}
