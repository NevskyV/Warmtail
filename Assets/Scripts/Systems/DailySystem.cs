using UnityEngine;
using Zenject;
using System;
using System.Globalization;
using Data;
using Data.Player;
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
            CheckTime();
        }

        public async void CheckTime()
        {
            DateTime timeNow = DateTime.UtcNow;

            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                using (UnityWebRequest request = UnityWebRequest.Head("https://www.google.com"))
                {
                    try
                    {
                        await request.SendWebRequest();

                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            string dateHeader = request.GetResponseHeader("Date");
                            if (!string.IsNullOrEmpty(dateHeader) && DateTime.TryParse(dateHeader, out DateTime netTime))
                            {
                                Debug.Log("Global time");
                                timeNow = netTime.ToUniversalTime();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            Debug.Log(timeNow);
            string timeLastGameStr = _globalData.Get<SavablePlayerData>().TimeLastGame;
            DateTime timeLastGame = DateTime.MinValue;

            if (!string.IsNullOrEmpty(timeLastGameStr))
            {
                DateTime.TryParse(timeLastGameStr, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out timeLastGame);
            }

            _globalData.Edit<SavablePlayerData>(data => data.TimeLastGame = timeNow.ToString(CultureInfo.InvariantCulture));

            DateTime adjustedNow = timeNow.AddHours(-3);
            DateTime adjustedLast = timeLastGame != DateTime.MinValue ? timeLastGame.AddHours(-3) : timeLastGame;

            if (adjustedNow.Date > adjustedLast.Date)
                OnDiscardedResources?.Invoke();
            else
                OnLoadedResources?.Invoke();
        }
    }
}