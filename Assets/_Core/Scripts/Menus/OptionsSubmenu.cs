using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsSubmenu : MainMenuSubmenu
{
    [Header("Configuration Controls")]
    [SerializeField]
    private UISlider shadowDistanceSlider;
    [SerializeField]
    private UISlider musicVolumeSlider;
    [SerializeField]
    private UISlider sfxVolumeSlider;
    [SerializeField]
    private UISlider uiVolumeSlider;
    [SerializeField]
    private UIButton applyButton;
    [SerializeField]
    private CanvasGroup[] menus;
    private IGameService game;
    private bool isConfigDirty;

    public void ChangeSettingsMenu(CanvasGroup menu)
    {
        SetConfigDirty(false);
        for (int i = 0; i < menus.Length; i++)
        {
            var m = menus[i];
            var isActive = m == menu;
            m.DOFade(isActive ? 1f : 0f, 0.25f);
            m.blocksRaycasts = isActive;
            m.interactable = isActive;
        }
    }
    public override IEnumerator EnterState()
    {
        game = Services.locator.Get<IGameService>();
        SetConfigDirty(false);
        LoadValuesIntoUI();
        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].alpha = 0f;
            menus[i].blocksRaycasts = false;
            menus[i].interactable = false;
        }
        return base.EnterState();
    }
    private void LoadValuesIntoUI()
    {
        var cfg = game.config;
        shadowDistanceSlider.value = cfg.shadowDistance;
        musicVolumeSlider.value = cfg.musicVolume;
        sfxVolumeSlider.value = cfg.sfxVolume;
        uiVolumeSlider.value = cfg.uiVolume;
    }
    protected override void Awake()
    {
        base.Awake();
        shadowDistanceSlider.onValueChanged.AddListener((v) =>
        {
            game.config.shadowDistance = v;
            SetConfigDirty();
        });
        musicVolumeSlider.onValueChanged.AddListener((v) =>
        {
            game.config.musicVolume = v;
            game.audioMixer.SetFloat("MusicVolume", v);
            SetConfigDirty();
        });
        sfxVolumeSlider.onValueChanged.AddListener((v) =>
        {
            game.config.sfxVolume = v;
            game.audioMixer.SetFloat("SFXVolume", v);
            SetConfigDirty();
        });
        uiVolumeSlider.onValueChanged.AddListener((v) =>
        {
            game.config.uiVolume = v;
            game.audioMixer.SetFloat("UIVolume", v);
            SetConfigDirty();
        });
        applyButton.onClick.AddListener(() =>
        {
            game.WriteConfiguration();
            SetConfigDirty(false);
        });
    }
    private void SetConfigDirty(bool dirty = true)
    {
        isConfigDirty = dirty;
        applyButton.interactable = dirty;
    }
}