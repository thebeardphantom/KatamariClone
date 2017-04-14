using Newtonsoft.Json;
using System.IO;
using UnityCommonLibrary.FSM;
using UnityCommonLibrary.Messaging;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityCommonLibrary;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour, IGameService
{
    public GameConfiguration config { get; set; }
    public Player player { get; set; }
    public AudioMixer audioMixer { get; private set; }
    public GameSession session { get; private set; }
    public UCLStateMachine<GameState> state { get; private set; }

    public void WriteConfiguration()
    {
        try
        {
            var json = JsonConvert.SerializeObject(config);
            File.WriteAllText(GameConfiguration.path, json);
            Messaging<GameMessage>.Broadcast(GameMessage.ConfigurationChanged);
        }
        catch (System.Exception e)
        {
            // LogError and LogException caused flow of execution to cease
            //Services.locator.Get<ILogger>().LogError("", e.Message);
        }
    }
    public void ReadConfiguration()
    {
        try
        {
            var configJson = File.ReadAllText(GameConfiguration.path);
            config = JsonConvert.DeserializeObject<GameConfiguration>(configJson);
            Messaging<GameMessage>.Broadcast(GameMessage.ConfigurationChanged);
        }
        catch (System.Exception e)
        {
            config = new GameConfiguration();
            WriteConfiguration();
            //Services.locator.Get<ILogger>().LogError("", e.Message);
        }
    }
    public void LoadLevel(string level)
    {
        Menu.ShowMenu<LoadScreen>();
        Messaging<GameMessage>.Broadcast(GameMessage.LoadRequested, new GenericMessageData<string>(level));
    }
    private void OnLevelLoadComplete(MessageData abstData)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            state.SwitchState(GameState.Playing);
            // Force state to enter immediately
            state.Tick();
        }
    }
    private void OnSessionEnded(MessageData abstData)
    {
        state.SwitchState(GameState.PostGame);
        Menu.ShowMenu<PostGameMenu>();
    }
    private void OnConfigurationChanged(MessageData abstData)
    {
        audioMixer.SetFloat("MusicVolume", config.musicVolume);
        audioMixer.SetFloat("SFXVolume", config.sfxVolume);
        audioMixer.SetFloat("UIVolume", config.uiVolume);
        QualitySettings.shadowDistance = config.shadowDistance;
    }
    private void Awake()
    {
        // Setup game-wide state machine
        state = new UCLStateMachine<GameState>();
        state.SetOnTick(GameState.Playing, () =>
        {
            if (GameInput.GetButtonDown(GameInput.PAUSE_ACTION))
            {
                session.duration = 0.001f;
            }
            session.Update();
        });
        state.SetOnEnter(GameState.Playing, (previousState) =>
        {
            session = new GameSession(TimeSpan.FromMinutes(3), 4.5f);
            Menu.ShowMenu<GameUI>();
            var requiredObjects = Resources.LoadAll<GameObject>("GameRequired");
            for (int i = 0; i < requiredObjects.Length; i++)
            {
                Instantiate(requiredObjects[i]);
            }
            // Allow Start to be called before starting session
            Jobs.ExecuteNextFrame(() => Messaging<GameMessage>.Broadcast(GameMessage.SessionStarted));
        });
        state.SetOnEnter(GameState.PostGame, (previousState) =>
        {
            player.enabled = false;
            player.rigidbody.isKinematic = true;
        });
        state.EngageMachine();
        audioMixer = Resources.Load<AudioMixer>("MainMixer");
        ReadConfiguration();
    }
    private void OnEnable()
    {
        Messaging<GameMessage>.Register(GameMessage.LevelLoadComplete, OnLevelLoadComplete);
        Messaging<GameMessage>.Register(GameMessage.SessionEnded, OnSessionEnded);
        Messaging<GameMessage>.Register(GameMessage.ConfigurationChanged, OnConfigurationChanged);
    }
    private void OnDisable()
    {
        Messaging<GameMessage>.RemoveAll(this);
    }
    private void Update()
    {
        state.Tick();
        Messaging<GameMessage>.Update();
    }
}