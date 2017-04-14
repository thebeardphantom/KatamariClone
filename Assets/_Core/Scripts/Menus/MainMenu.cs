using System;
using System.Collections;
using DG.Tweening;
using UnityCommonLibrary.Messaging;
using UnityEngine;
using UnityCommonLibrary;
using UnityEngine.EventSystems;

public class MainMenu : Menu
{
    [Header("Main Menu")]
    [SerializeField]
    private MainMenuSubmenu initialSubmenu;
    private MainMenuSubmenu currentMenu;
    private CanvasGroup canvasGroup;
    private Coroutine changeSubmenuRoutine;

    public void GoToSubmenu(MainMenuSubmenu menu)
    {
        if (changeSubmenuRoutine == null)
        {
            changeSubmenuRoutine = StartCoroutine(ChangeSubmenu(menu));
        }
    }
    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Messaging<GameMessage>.Register(
            GameMessage.LoadRequested,
            (d) =>
            {
                canvasGroup.DOFade(0f, 4f).OnComplete(() => Messaging<GameMessage>.Broadcast(GameMessage.LoadingReady, d));
            }
        );
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Messaging<GameMessage>.RemoveAll(this);
    }
    private IEnumerator Start()
    {
        Services.locator.Get<IGameService>().state.SwitchState(GameState.MainMenu);
        yield return new WaitForSeconds(1f);
        GoToSubmenu(initialSubmenu);
    }
    private IEnumerator ChangeSubmenu(MainMenuSubmenu menu)
    {
        // Deselect all, exit current then enter next
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;
        if (currentMenu != null)
        {
            yield return StartCoroutine(currentMenu.ExitState());
        }
        currentMenu = menu;
        yield return StartCoroutine(currentMenu.EnterState());
        EventSystem.current.SetSelectedGameObject(null);
        changeSubmenuRoutine = null;
    }
}