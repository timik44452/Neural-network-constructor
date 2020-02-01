using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MouseMode
{
    None,
    Drag,
    Hold,
    Click
}

[System.Serializable]
public class InputController
{
    public Action OnMouseDown;
    public Action OnMouseUp;
    public Action<KeyCode> OnKeyDown;
    public Action<KeyCode> OnKeyUp;

    public MouseMode mouseMode { get; private set; }

    public bool isScroll { get; private set; }
    public bool isControl { get; private set; }
    public bool isShift { get; private set; }

    public bool isLeftMouse { get; private set; }
    public bool isRightMouse { get; private set; }
    public bool isMiddleMouse { get; private set; }

    public Vector2 mousePosition { get; private set; }
    public Vector2 delta { get; private set; }


    public void Update()
    {
        Event eventCurrent = Event.current;

        mousePosition = eventCurrent.mousePosition;

        if (eventCurrent.type == EventType.MouseDown)
        {
            isLeftMouse |= eventCurrent.button == 0;
            isRightMouse |= eventCurrent.button == 1;
            isMiddleMouse |= eventCurrent.button == 2;

            OnMouseDown?.Invoke();
        }

        if (eventCurrent.type == EventType.MouseUp)
        {
            isLeftMouse &= !(eventCurrent.button == 0);
            isRightMouse &= !(eventCurrent.button == 1);
            isMiddleMouse &= !(eventCurrent.button == 2);

            OnMouseUp?.Invoke();
        }

        if(eventCurrent.type == EventType.KeyDown)
        {
            OnKeyDown?.Invoke(eventCurrent.keyCode);
        }

        if (eventCurrent.type == EventType.KeyUp)
        {
            OnKeyUp?.Invoke(eventCurrent.keyCode);
        }

        delta = eventCurrent.delta;
        isShift = eventCurrent.shift;
        isControl = eventCurrent.control;
        isScroll = eventCurrent.isScrollWheel;
    }

    public void ResetMouseButtons()
    {
        isLeftMouse = false;
        isRightMouse = false;
        isMiddleMouse = false;
    }
}
