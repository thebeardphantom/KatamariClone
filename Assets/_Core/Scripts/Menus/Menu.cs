using UnityEngine;
using System.Collections.Generic;

public abstract class Menu : MonoBehaviour
{
    private static readonly HashSet<Menu> openedMenus = new HashSet<Menu>();
    public static Camera uiCamera;

    [Header("Menu")]
    [SerializeField]
    private bool singleMenu = true;
    protected Canvas canvas;

    public static T ShowMenu<T>() where T : Menu
    {
        foreach (var menu in openedMenus)
        {
            var found = menu as T;
            if (found != null)
            {
                return found;
            }
        }
        return Instantiate(Resources.LoadAll<T>("Menus")[0]);
    }
    protected virtual void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = uiCamera;
        canvas.planeDistance = uiCamera.farClipPlane / 2f;
    }
    protected virtual void OnEnable()
    {
        openedMenus.Add(this);
    }
    protected virtual void OnDisable()
    {
        openedMenus.Remove(this);
    }
}
