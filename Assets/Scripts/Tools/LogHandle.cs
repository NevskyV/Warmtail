using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class LogHandle : MonoBehaviour
{
//#if UNITY_EDITOR
    [SerializeField] private Text txt;
    [SerializeField] private GameObject image;

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy() { 
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception)
        {
            image.SetActive(true);
            txt.text += "/n" + logString;
            txt.text += " " + stackTrace;
        }
    }

    public void Dismiss()
    {
        image.SetActive(false);
    }
//#endif
}