using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityCommonLibrary.Messaging;
using UnityCommonLibrary;
using UnityEngine.UI;
using System;

public class PostGameMenu : Menu
{
    [SerializeField]
    private GameObject objTextTemplate;
    [SerializeField]
    private Text winLose;
    [SerializeField]
    private RectTransform list;

    public void GoToMenu()
    {
        Services.locator.Get<IGameService>().LoadLevel("MainMenu");
    }
    public void Restart()
    {
        Services.locator.Get<IGameService>().LoadLevel(SceneManager.GetActiveScene().name);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        Messaging<GameMessage>.Register(
            GameMessage.LoadRequested,
            (abstData) =>
            {
                Messaging<GameMessage>.Broadcast(
                    GameMessage.LoadingReady,
                    abstData
                );
            }
        );
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Messaging<GameMessage>.RemoveAll(this);
    }
    private void Start()
    {
        var game = Services.locator.Get<IGameService>();
        var didWin = game.session.targetRadius < game.player.katamariAverageRadius;
        winLose.text = didWin ? "You Win!" : "You Lose!";
        winLose.color = didWin ? Color.green : Color.red;
        StartCoroutine(DisplayAllObjects());
    }

    private IEnumerator DisplayAllObjects()
    {
        var player = Services.locator.Get<IGameService>().player;
        var objs = player.attachedObjects;
        objs.Sort((obj1, obj2) => obj1.bounds.size.sqrMagnitude.CompareTo(obj2.bounds.size.sqrMagnitude));
        var lookup = new Dictionary<string, int>();
        for (int i = 0; i < objs.Count; i++)
        {
            if (lookup.ContainsKey(objs[i].displayName))
            {
                lookup[objs[i].displayName]++;
            }
            else
            {
                lookup.Add(objs[i].displayName, 1);
            }
        }
        objTextTemplate.gameObject.SetActive(false);
        foreach (var kvp in lookup)
        {
            var template = Instantiate(objTextTemplate, list);
            template.SetActive(true);
            var label = template.GetComponent<Text>();
            label.text = string.Format("{0} x{1}", kvp.Key, kvp.Value);
            yield return new WaitForSecondsRealtime(0.125f);
        }
    }
}