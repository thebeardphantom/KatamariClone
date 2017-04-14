using DG.Tweening;
using System;
using UnityCommonLibrary;
using UnityCommonLibrary.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : Menu
{
    [SerializeField]
    private Text sizeLabel;
    [SerializeField]
    private Image timeBackground;
    [SerializeField]
    private Text timeLabel;
    [SerializeField]
    private CanvasGroup objPreview;
    [SerializeField]
    [Range(0f, 1f)]
    private float objPreviewInset = 0.5f;
    [SerializeField]
    private RectTransform objPreviewBoundsSource;
    [SerializeField]
    private Text objLabel;
    [SerializeField]
    private Renderer objRenderer;
    [SerializeField]
    private MeshFilter objMeshFilter;

    private Quaternion initialObjRendererRotation;
    private IGameService game;

    protected override void Awake()
    {
        base.Awake();
        objPreview.alpha = 0f;
        initialObjRendererRotation = objRenderer.transform.rotation;
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Messaging<GameMessage>.Register(GameMessage.KatamariBoundsUpdated, OnObjectPickedUp);
        Messaging<GameMessage>.Register(GameMessage.SessionEndingSoon, OnSessionEndingSoon);
        Messaging<GameMessage>.Register(GameMessage.SessionEnded, OnSessionEnded);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Messaging<GameMessage>.RemoveAll(this);
    }
    private void OnSessionEnded(MessageData abstData)
    {
        gameObject.SetActive(false);
    }
    private void OnSessionEndingSoon(MessageData abstData)
    {
        if (game.player.katamariAverageRadius < game.session.targetRadius)
        {
            timeBackground.transform.DOPunchScale(Vector3.one * 0.1f, 1f).SetLoops(10);
            timeBackground.DOColor(Color.red, 1f).From().SetLoops(10);
        }
    }
    private void OnObjectPickedUp(MessageData abstData)
    {
        var player = Services.locator.Get<IGameService>().player;
        sizeLabel.text = string.Format("{0:0.00}m / {1}m", player.katamariAverageRadius * 2f, game.session.targetRadius * 2f);
        if (player.katamariAverageRadius >= game.session.targetRadius)
        {
            DOTween.Kill(timeBackground.transform, true);
            DOTween.Kill(timeBackground, true);
            timeBackground.color = Color.green;
        }
        if (abstData == null)
        {
            return;
        }
        objPreview.alpha = 1f;

        var obj = (abstData as GenericMessageData<KatamariObject>).value;
        objLabel.text = obj.displayName;
        objMeshFilter.sharedMesh = obj.preview.mesh;
        objRenderer.sharedMaterial = obj.preview.material;

        // Resize renderer to fit into object preview bounds
        var world = new Vector3[4];
        objPreviewBoundsSource.GetWorldCorners(world);
        var objPreviewBoundsSize = Vector3.Distance(world[0], world[2]) * objPreviewInset;
        var objPreviewBounds = new Bounds((world[0] + world[1] + world[2] + world[3]) / 4f, Vector3.one * objPreviewBoundsSize);
        objRenderer.transform.localScale = Vector3.one;
        objRenderer.transform.rotation = initialObjRendererRotation;
        var objBounds = objRenderer.bounds;
        var maxRatio = Mathf.Min(
            objPreviewBoundsSize / objBounds.size.x,
            objPreviewBoundsSize / objBounds.size.y,
            objPreviewBoundsSize / objBounds.size.z
        );
        objRenderer.transform.localScale = Vector3.one * maxRatio;
    }
    private void Start()
    {
        game = Services.locator.Get<IGameService>();
        OnObjectPickedUp(null);
    }
    private void Update()
    {
        if (game.session.timeLeft >= 0f)
        {
            var span = TimeSpan.FromSeconds(game.session.timeLeft);
            timeLabel.text = string.Format("{0:00}:{1:00}.{2:00}", span.Minutes, span.Seconds, span.Milliseconds);
        }
        else
        {
            timeLabel.text = "00:00.00";
        }
    }
}
