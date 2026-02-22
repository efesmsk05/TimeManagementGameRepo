using System;
using System.Collections.Generic;

public static class EventBus
{
    // Tek bir sözlük yeterli!
    private static readonly Dictionary<Type, Delegate> Events = new Dictionary<Type, Delegate>();

    public static void Subscribe<T>(Action<T> listener) where T : struct
    {
        var type = typeof(T);
        if (!Events.ContainsKey(type)) Events[type] = null;
        Events[type] = Delegate.Combine(Events[type], listener);
    }

    public static void Unsubscribe<T>(Action<T> listener) where T : struct
    {
        var type = typeof(T);
        if (Events.ContainsKey(type))
        {
            var currentDel = Events[type];
            Events[type] = Delegate.Remove(currentDel, listener);
            if (Events[type] == null) Events.Remove(type);
        }
    }

    public static void Publish<T>(T eventArgs) where T : struct
    {
        var type = typeof(T);
        if (Events.TryGetValue(type, out var del))
        {
            if (del is Action<T> callback) callback.Invoke(eventArgs);
        }
    }
}