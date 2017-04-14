using DG.Tweening;
using System.Collections;
using UnityCommonLibrary.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScreen : Menu
{
    private const float ANIM_START_ALPHA = 0.5f;

    [Header("Load Screen")]
    [SerializeField]
    private new Image animation;
    [SerializeField]
    private Sprite[] animationFrames;
    private CanvasGroup group;
    private int index;
    private float animationTimer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        group = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        group.alpha = 0f;
        animation.color = new Color(1f, 1f, 1f, 0f);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Messaging<GameMessage>.Register(
            GameMessage.LoadingReady,
            (d) =>
            {
                // Fade in canvas, then start loading
                var sequence = DOTween.Sequence();
                sequence.Append(group.DOFade(1f, 1f));
                sequence.Append(animation.DOFade(ANIM_START_ALPHA, 1f));
                sequence.AppendCallback(() => StartCoroutine(LoadLevelRoutine((d as GenericMessageData<string>).value)));
            }
        );
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Messaging<GameMessage>.RemoveAll(this);
    }
    private IEnumerator LoadLevelRoutine(string level)
    {
        // Unload current scene
        var game = Services.locator.Get<IGameService>();
        game.state.SwitchState(GameState.Loading);
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        // Begin load new scene
        var logger = Services.locator.Get<ILogger>();
        logger.LogFormat(LogType.Log, "Loading level: {0}", level);
        var load = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        while (!load.isDone)
        {
            // Lerp animation color to indicate progress
            animation.color = Color.Lerp(new Color(1f, 1f, 1f, ANIM_START_ALPHA), Color.white, load.progress);
            yield return null;
        }
        logger.LogFormat(LogType.Log, "{0} SCENE LOAD ROUTINE COMPLETE", Time.realtimeSinceStartup);
        Messaging<GameMessage>.Broadcast(GameMessage.LevelLoadComplete, new GenericMessageData<string>(level));
        // Fade out canvas
        yield return group.DOFade(0f, 1f).OnComplete(() => Destroy(gameObject)).WaitForCompletion();
    }
    private void Update()
    {
        const float FRAMERATE = 1f / 60f;
        if (Time.realtimeSinceStartup - animationTimer >= FRAMERATE)
        {
            animation.sprite = animationFrames[index];
            index++;
            index %= animationFrames.Length;
            animationTimer = Time.realtimeSinceStartup;
        }
    }
}
