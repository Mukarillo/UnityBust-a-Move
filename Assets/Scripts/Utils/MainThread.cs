using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class MainThread : MonoBehaviour
{
    private static readonly Queue<Action> invokables = new Queue<Action>();

    public static MainThread component { get; private set; }

    public static MainThread Initiate()
    {
        component = new GameObject("MainThread").AddComponent<MainThread>();
        DontDestroyOnLoad(component.gameObject);
        return component;
    }        

    public static void Invoke(Action action)
    {
        if (IsMainThread()) action.Invoke();
        else invokables.Enqueue(action);
    }

    private static Thread mainThreadRef;
	public static void SetMainThread(){
		mainThreadRef = Thread.CurrentThread;	
	} 
	public static bool IsMainThread(){
		return mainThreadRef.Equals(Thread.CurrentThread);
	}   

    private void Dispatch()
    {
        while (invokables.Count > 0)
        {
            var a = invokables.Dequeue();
            if (a != null) a.Invoke();
			else Debug.Log("null invokable");
        }
    }

    public void Update()
    {
        Dispatch();
    }

    public void OnDestroy()
    {
        Dispatch();
    }
}