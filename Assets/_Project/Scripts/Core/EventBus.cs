
using System;
using System.Collections.Generic;

public static class EventBus
{
    private static class EventBinding<T> where T : struct
    {
        private static readonly Dictionary<Type, Delegate> bindings = new Dictionary<Type, Delegate>();

        public static void Add(Action<T> listener)
        {
            Type key = typeof(T);

            if (bindings.ContainsKey(key))
            {
                bindings[key] = Delegate.Combine(bindings[key], listener);
            }
            else
            {
                bindings[key] = listener;
            }
        }

        public static void Remove(Action<T> listener)
        {
            Type key = typeof(T);

            if (bindings.ContainsKey(key))
            {
                Delegate current = Delegate.Remove(bindings[key], listener);

                if (current == null)
                {
                    bindings.Remove(key);
                }
                else
                {
                    bindings[key] = current;
                }
            }
        }

        public static void Invoke(T eventData)
        {
            Type key = typeof(T);

            if (bindings.ContainsKey(key))
            {
                Action<T> action = bindings[key] as Action<T>;

                if (action != null)
                {
                    action.Invoke(eventData);
                }
            }
        }

        public static void Clear()
        {
            bindings.Clear();
        }
    }

    public static void Subscribe<T>(Action<T> listener) where T : struct
    {
        EventBinding<T>.Add(listener);
    }

    public static void Unsubscribe<T>(Action<T> listener) where T : struct
    {
        EventBinding<T>.Remove(listener);
    }

    public static void Publish<T>(T eventData) where T : struct
    {
        EventBinding<T>.Invoke(eventData);
    }

    public static void ClearAll<T>() where T : struct
    {
        EventBinding<T>.Clear();
    }
}