using System.Collections;
using UnityEngine;

public sealed class Coroutines : MonoBehaviour
{
    private static Coroutines instanse
    {
        get
        {
            if (_instance == null)
            {
                var gameObject = new GameObject("COROUTINE MANAGER");
                _instance = gameObject.AddComponent<Coroutines>();
                DontDestroyOnLoad(gameObject);
            }
            return _instance;
        }
    }
    private static Coroutines _instance;

    public static Coroutine StartRoutine(IEnumerator enumerator)
    {
        return instanse.StartCoroutine(enumerator);
    }
    public static void StopRoutine(Coroutine routine)
    {
        if(routine != null)
        {
            instanse.StopCoroutine(routine);
        }
    }
}