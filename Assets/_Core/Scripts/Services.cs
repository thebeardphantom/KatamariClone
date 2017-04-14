using UnityCommonLibrary;
using UnityCommonLibrary.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for all required object initialization.
/// </summary>
public static class Services
{
    public class DebugServiceLocator : ServiceLocator
    {
        protected override void RegisterServices()
        {
            Register(Debug.logger);
            RegisterNewBehaviour<IGameService, Game>();
        }
    }
    public class ReleaseServiceLocator : ServiceLocator
    {
        protected override void RegisterServices()
        {
            Register<ILogger, NullLogger>();
            RegisterNewBehaviour<IGameService, Game>();
        }
    }

    public static ServiceLocator locator { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnGameStart()
    {
        // Initiate objects that are always needed
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Prefabs/UIInput")));
        Menu.uiCamera = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UICamera")).GetComponent<Camera>();
        Object.DontDestroyOnLoad(Menu.uiCamera);
        if (SceneManager.GetActiveScene().path.Contains("/Test/"))
        {
            // Services aren't required in test scenes
            return;
        }
        ReloadServices();
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            // Immediately fire LevelLoadComplete msg if game starts in a map
            // Allows for instant testing of any scene
            Messaging<GameMessage>.BroadcastImmediate(
                GameMessage.LevelLoadComplete,
                new GenericMessageData<string>(SceneManager.GetActiveScene().name)
            );
        }
    }
    public static void ReloadServices()
    {
        if (Application.isEditor || Debug.isDebugBuild)
        {
            locator = new DebugServiceLocator();
        }
        else
        {
            locator = new ReleaseServiceLocator();
        }
        Messaging<GameMessage>.BroadcastImmediate(GameMessage.ServicesRegistered);
    }
}