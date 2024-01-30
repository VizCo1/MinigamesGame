using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ComputerControllerUI : MonoBehaviour
{
    private class ApplicationIcons
    {
        public DesktopIcon DesktopIcon { get; set; }
        public BarIcon BarIcon { get; set; }
    }

    private class ApplicationInformation
    {
        public DesktopIcon DesktopIcon { get; private set; }
        public BarIcon BarIcon { get; private set; }
        public Window Window { get; private set; }

        public ApplicationInformation(DesktopIcon desktopIcon, BarIcon barIcon, Window window)
        {
            DesktopIcon = desktopIcon;
            BarIcon = barIcon;
            Window = window;
        }
    }

    private enum Computerstate
    {
        IsDesktop,
        IsWindow,
    }

    private Computerstate _state = Computerstate.IsDesktop;

    private Dictionary<DesktopIcon, ApplicationInformation> _desktopIconAppInfoDictionary = new Dictionary<DesktopIcon, ApplicationInformation>();
    private Dictionary<BarIcon, ApplicationInformation> _barIconAppInfoDictionary = new Dictionary<BarIcon, ApplicationInformation>();

    private Canvas _mainCanvas;
    private Pool _windowPool;

    [SerializeField] private BarUI _barUI;
    
    private ApplicationIcons _currentApplicationIcons;

    public static ComputerControllerUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        } 

        Instance = this;
        _mainCanvas = GetComponent<Canvas>();
        _windowPool = GetComponent<Pool>();
        _currentApplicationIcons = new ApplicationIcons();
    }

    private void Update()
    {
        //Debug.Log(_state.ToString());
    }

    #region Handle Icon Clicked

    public void HandleDesktopIconClicked(DesktopIcon desktopIcon)
    {
        // Icons can open a window when in desktop
        if (!IsDesktopState())
        {
            return;
        }

        // Icon already has a window
        if (_desktopIconAppInfoDictionary.ContainsKey(desktopIcon))
        {
            // Since it has a window, the value is in the dictionary
            _desktopIconAppInfoDictionary.TryGetValue(desktopIcon, out ApplicationInformation appInfo); 

            // Current app icons
            _currentApplicationIcons.BarIcon = appInfo.BarIcon;
            _currentApplicationIcons.DesktopIcon = desktopIcon;

            OpenWindowWithDesktopIcon(desktopIcon);
        }
        // Icon does not have a window
        else if (_windowPool.TryDequeue(out GameObject windowGO))
        {
            Window window = windowGO.GetComponent<Window>();

            // Current app icons
            _currentApplicationIcons.DesktopIcon = desktopIcon;
            _currentApplicationIcons.BarIcon = _barUI.AddIcon().GetComponent<BarIcon>();

            // Add to dictionaries
            ApplicationInformation appInfo = new ApplicationInformation(desktopIcon, _currentApplicationIcons.BarIcon, window);
            _desktopIconAppInfoDictionary.Add(_currentApplicationIcons.DesktopIcon, appInfo);
            _barIconAppInfoDictionary.Add(_currentApplicationIcons.BarIcon, appInfo);

            OpenWindowWithDesktopIcon(desktopIcon);
        }
        // No windows left in the window pool
        else
        {
            // Current app icons
            _currentApplicationIcons.DesktopIcon = desktopIcon;
            _currentApplicationIcons.BarIcon = _barUI.AddIcon().GetComponent<BarIcon>();

            // Create new window
            Window window = _windowPool.CreateGameObject().GetComponent<Window>();

            // Add to dictionaries
            ApplicationInformation appInfo = new ApplicationInformation(desktopIcon, _currentApplicationIcons.BarIcon, window);
            _desktopIconAppInfoDictionary.Add(desktopIcon, appInfo);
            _barIconAppInfoDictionary.Add(_currentApplicationIcons.BarIcon, appInfo);

            OpenWindowWithDesktopIcon(desktopIcon);
        }
    }

    public void HandleBarIconClicked(BarIcon barIcon)
    {
        // Icons can open a window when in desktop
        if (!IsDesktopState())
        {
            return;
        }

        // Icon has a window
        if (_barIconAppInfoDictionary.ContainsKey(barIcon))
        {
            // Since it has a window, the value is in the dictionary
            _barIconAppInfoDictionary.TryGetValue(barIcon, out var appInfo);

            // Current app icons
            _currentApplicationIcons.DesktopIcon = appInfo.DesktopIcon;
            _currentApplicationIcons.BarIcon = barIcon;

            OpenWindowWithBarIcon(barIcon);
        }
        else
        {
            Debug.LogError("This bar icon should not be activated, it does not have a window");
        }
    }

    #endregion

    #region Open and Close Window

    private void OpenWindowWithDesktopIcon(DesktopIcon icon)
    {
        if (_desktopIconAppInfoDictionary.TryGetValue(icon, out ApplicationInformation appInfo))
        {
            appInfo.Window.gameObject.SetActive(true);
            SetIsWindowState();
            appInfo.Window.Open();
        }
        else
        {
            Debug.LogError("Error when trying to open a window, not found in dictionary");
        }
    }

    private void OpenWindowWithBarIcon(BarIcon icon)
    {
        if (_barIconAppInfoDictionary.TryGetValue(icon, out ApplicationInformation appInfo))
        {
            appInfo.Window.gameObject.SetActive(true);
            SetIsWindowState();
            appInfo.Window.Open();
        }
        else
        {
            Debug.LogError("Error when trying to open a window, not found in dictionary");
        }
    }

    public void CloseWindow(Window window)
    {
        window.ToDefault();

        _windowPool.Enqueue(window.gameObject);

        _barUI.RemoveIcon(_currentApplicationIcons.BarIcon);
        _barUI.FixAllPositions(_currentApplicationIcons.BarIcon);

        // Remove from dictionaries
        _desktopIconAppInfoDictionary.Remove(_currentApplicationIcons.DesktopIcon);
        _barIconAppInfoDictionary.Remove(_currentApplicationIcons.BarIcon);


        SetIsDesktopState();
    }

    #endregion

    public Canvas GetMainCanvas() => _mainCanvas;

    public bool IsDesktopState() => _state == Computerstate.IsDesktop;
    public bool IsWindowState() => _state == Computerstate.IsWindow;
    public void SetIsDesktopState() => _state = Computerstate.IsDesktop;
    public void SetIsWindowState() => _state = Computerstate.IsWindow;
}
