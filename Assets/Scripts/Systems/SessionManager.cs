using System;
using System.IO;
using Data;
using UnityEngine;
using Zenject;

namespace Systems
{
   
    public class SessionSystem : ITickable, IDisposable
    {
        private SessionData _currentSession;
        private float _startTime;

        public SessionSystem()
        {
            _startTime = Time.realtimeSinceStartup;
            _currentSession = new SessionData();
        }

        public void Tick()
        {
            _currentSession.Playtime = Time.realtimeSinceStartup - _startTime;
        }

        public void AddShell()
        {
            Debug.Log("Session add shell " );
            _currentSession.ShellsCollected++;
        }

        public void AddItemBought()
        {
            Debug.Log("Session add item bought " );
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
}