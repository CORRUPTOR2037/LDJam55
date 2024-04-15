using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInstancesHolder
{
    private static Dictionary<Type, object> instances = new Dictionary<Type, object>();
    public static void Register(MonoBehaviour obj)
    {
        instances[obj.GetType()] = obj;
    }

    public static void Reset() => instances.Clear();

    public static T Get<T>() => (T) instances[typeof(T)];
}
