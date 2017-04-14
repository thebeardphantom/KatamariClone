using UnityCommonLibrary.FSM;
using UnityEngine.Audio;

public enum GameState
{
    MainMenu,
    Loading,
    Playing,
    Paused,
    PostGame,
}

public interface IGameService
{
    AudioMixer audioMixer { get; }
    GameConfiguration config { get; set; }
    Player player { get; set; }
    GameSession session { get; }
    UCLStateMachine<GameState> state { get; }

    void WriteConfiguration();
    void ReadConfiguration();
    void LoadLevel(string name);
}