using DG.Tweening;
using UnityEngine;

public class LevelSelectSubmenu : MainMenuSubmenu
{
    [SerializeField]
    private AudioSource startGameAudio;
    [SerializeField]
    private AudioSource musicAudio;

    public void PlayGame(string level)
    {
        musicAudio.DOFade(0f, 0.5f);
        startGameAudio.Play();
        Services.locator.Get<IGameService>().LoadLevel(level);
    }
}