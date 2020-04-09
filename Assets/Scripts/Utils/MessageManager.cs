using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Event
{
    ClickOnCell,
}

public class MessageManager : Singleton<MessageManager>
{
    Dictionary<Event, List<Action<object[]>>> m_EventHandler;

    protected override void OnInit()
    {
        base.OnInit();

        m_EventHandler = new Dictionary<Event, List<Action<object[]>>>();
    }

    public void Register(Event targetEvent, Action<object[]> action)
    {
        List<Action<object[]>> targetHandlers;
        if(m_EventHandler.TryGetValue(targetEvent, out targetHandlers))
        {
            targetHandlers.Add(action);
        }
        else
        {
            targetHandlers = new List<Action<object[]>>();
            targetHandlers.Add(action);

            m_EventHandler.Add(targetEvent, targetHandlers);
        }
    }

    public void UnRegister() { }

    public void Do(Event targetEvent, params object[] param)
    {
        List<Action<object[]>> targetHandlers;
        if (m_EventHandler.TryGetValue(targetEvent, out targetHandlers))
        {
            foreach(Action<object[]> action in targetHandlers)
            {
                action?.Invoke(param);
            }
        }
    }

}
