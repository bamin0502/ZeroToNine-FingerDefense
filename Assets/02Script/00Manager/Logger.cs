//custom Logger
using UnityEngine;

public class Logger : MonoBehaviour
{
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
    public static void Log(object message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }
    public static void Log(string message, GameObject gameObject)
    {
#if UNITY_EDITOR
        Debug.Log(message, gameObject);
#endif
    }
    
    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message); 
#endif
        
    }

    public static void LogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
        
        
    }

    
}