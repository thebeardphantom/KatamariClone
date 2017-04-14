using UnityCommonLibrary;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : Button
{
    public AudioCue onSelectCue;
    public AudioCue onClickCue;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        onSelectCue.PlayOneShot();
    }
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onClickCue.PlayOneShot();
        EventSystem.current.SetSelectedGameObject(null, eventData);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Select();
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        EventSystem.current.SetSelectedGameObject(null, eventData);
    }
}