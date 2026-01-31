using UnityEngine;
using Zenject;
using System;
using System.Globalization;
using Data;
using Data.Player;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Systems
{
    public class DailySystem : IInitializable
    {
        [Inject] private GlobalData _globalData;
        public static Action OnDiscardedResources = delegate {};
        public static Action OnLoadedResources = delegate {};

        public void Initialize()
        {
            //CheckTime();
        }

        public async void CheckTime()
        {
            Debug.Log("Send time request"); 
            string url = "https://worldtimeapi.org/api/timezone/Etc/UTC";
            DateTime timeNow = DateTime.UtcNow.AddHours(3);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();
            
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    JObject timeData = JObject.Parse(json);
                    long unixTime = (long)timeData["unixtime"];
                    timeNow = DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime.AddHours(3);
                
                    Debug.Log("Глобальное UTC время: " + timeNow); 
                }
                else
                {
                    Debug.LogError("Ошибка запроса: " + request.error);
                }
            }
            
            string timeLastGameStr = _globalData.Get<SavablePlayerData>().TimeLastGame;
            DateTime timeLastGame = new (2000, 1, 1, 0, 0, 0, 0);
            if (!string.IsNullOrEmpty(timeLastGameStr)) timeLastGame = DateTime.Parse(timeLastGameStr);
            
            _globalData.Edit<SavablePlayerData>(data => data.TimeLastGame = timeNow.ToString(CultureInfo.InvariantCulture));
            if (timeLastGame.Day != timeNow.Day || timeLastGame.Month != timeNow.Month || timeLastGame.Year != timeNow.Year)
                OnDiscardedResources?.Invoke();
            else
                OnLoadedResources?.Invoke();
        }
    }
}