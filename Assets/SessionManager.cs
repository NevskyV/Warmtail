using System;
using System.IO;
using UnityEngine;
using Zenject;

public class SessionManager : ITickable, IDisposable
{
    private SessionData _currentSession;
    private float _startTime;

    public SessionManager()
    {
        _startTime = Time.time;
        _currentSession = new SessionData();
    }

    public void Tick()
    {
        _currentSession.Playtime = Time.time - _startTime;
    }

    public void AddShell()
    {
        _currentSession.ShellsCollected++;
    }

    public void AddItemBought()
    {
        _currentSession.ItemsBought++;
    }

    public void SaveSession()
    {
        string json = JsonUtility.ToJson(_currentSession, true);

        string folder = Application.persistentDataPath + "/Sessions";
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string fileName = "session_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".json";
        string path = Path.Combine(folder, fileName);

        File.WriteAllText(path, json);

        Debug.Log("Session saved: " + path);
    }

    public void Dispose()
    {
        SaveSession();
    }
}