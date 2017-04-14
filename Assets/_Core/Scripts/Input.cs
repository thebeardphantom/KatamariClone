using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// A very simple class for accessing input.
/// </summary>
public static class GameInput
{
    public const string PAUSE_ACTION = "Pause";
    public const string MOVE_ACTION = "Move";
    public const string STRAFE_ACTION = "Strafe";
    public const string ROTATE_ACTION = "Rotate";
    public static bool enabled;

    public static float GetAxis(string axis)
    {
        return enabled ? Input.GetAxis(axis) : 0f;
    }
    public static float GetAxisRaw(string axis)
    {
        return enabled ? Input.GetAxisRaw(axis) : 0f;
    }
    public static bool GetButton(string btn)
    {
        return enabled ? Input.GetButton(btn) : false;
    }
    public static bool GetButtonDown(string btn)
    {
        return enabled ? Input.GetButtonDown(btn) : false;
    }
    public static bool GetButtonUp(string btn)
    {
        return enabled ? Input.GetButtonUp(btn) : false;
    }
}
