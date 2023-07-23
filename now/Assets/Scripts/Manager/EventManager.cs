using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 옵저버 패턴
/// </summary>
public abstract class Signal
{
    protected static List<Action> ListenersContainers { get; } = new(64);

    public static void Clear()
    {
        foreach (var listenersContainer in ListenersContainers)
        {
            listenersContainer.Invoke();
        }
    }
}

public abstract class Signal<TArg, TKind> : Signal
{
    private static readonly Dictionary<Type, Action<TArg>> Listeners = new();

    public Signal()
    {
        ListenersContainers.Add(Listeners.Clear);
    }

    // 리스너 추가
    public static void AddListener(Action<TArg> listener)
    {
        var type = typeof(TKind);
        if (!Listeners.TryGetValue(type, out var l))
        {
            Listeners.Add(type, listener);
        }
        else
        {
            Listeners[type] = l + listener;
        }
    }

    // 리스너 삭제
    public static void RemoveListener(Action<TArg> listener)
    {
        var type = typeof(TKind);
        if (Listeners.TryGetValue(type, out var l))
        {
            Listeners[type] = l - listener;
        }
    }

    // 리스너에 보낼수 있는지 체크
    public static bool CanDispatch()
    {
        var type = typeof(TKind);
        return Listeners.ContainsKey(type);
    }

    // 이벤트 보내기
    public static void Dispatch(TArg value)
    {
        var type = typeof(TKind);
        Listeners[type].Invoke(value);
    }

    public static bool TryDispatch(TArg value)
    {
        var type = typeof(TKind);
        if (!Listeners.TryGetValue(type, out var l)) return false;
        l.Invoke(value);
        return true;
    }
}

//public abstract class Signal<TKind> : Signal<INullable, TKind>
//{
//    static readonly Dictionary<Action, Action<INullable>> _listeners = new(64);
//
//    public static void AddListener(Action listener)
//    {
//        Action<INullable> temp = _ => listener();
//        _listeners[listener] = temp;
//        AddListener(temp);
//    }
//
//    public static void RemoveListener(Action listener)
//    {
//        if (_listeners.TryGetValue(listener, out var l))
//        {
//            RemoveListener(l);
//            _listeners.Remove(listener);
//        }
//    }
//
//    public static void Dispatch()
//    {
//        Dispatch(null);
//    }
//}

// 이벤트 매니저
public static class EventManager
{
    public class RegistModel : Signal<IModel, RegistModel> { }
    public class UnRegistModel : Signal<IModel, UnRegistModel> { }
    public class RegistVModel : Signal<(IModel, VModel), RegistModel> { }
    public class UnRegistVModel : Signal<GameObject, UnRegistVModel> { }
    public class CreateModel : Signal<IModel, CreateModel> { }

    public class Damage : Signal<int, Damage> { }
    public class GameStart : Signal<bool, GameStart> { }
    public class InGameStart : Signal<bool, InGameStart> { }
    public class GameEnd : Signal<bool, GameEnd> { }
}