using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using UnityEngine;
using Zenject;

namespace Systems
{

    public class WarmthSystem
    {
        private const int _warmthIncreaseRate = 1;
        private const float _increaseIntervalSeconds = 1f;
        private const float _cooldownSeconds = 3f;

        private GlobalData _globalData;

        private ResettableTimer _cooldownTimer;
        private CancellationTokenSource _increaseCts;
        private bool _isIncreasing;

        [Inject]
        private void Construct(GlobalData globalData)
        {
            _globalData = globalData;
            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentWarmth = _globalData.Get<SavablePlayerData>().Stars * 10;
            });
            _globalData.SubscribeTo<SavablePlayerData>(StartIncreaseIfNotRunning);
        }

        public void DecreaseWarmth(int value)
        {
            _globalData.Edit<RuntimePlayerData>(data =>
            {
                data.CurrentWarmth = Mathf.Max(data.CurrentWarmth - value, 0);
            });
            
            _increaseCts?.Cancel();
            _increaseCts?.Dispose();
            _increaseCts = null;
            _isIncreasing = false;
            //Debug.Log("decrease");
            _cooldownTimer ??= new ResettableTimer(_cooldownSeconds, StartIncreaseIfNotRunning);
            _cooldownTimer.Start();
        }

        private void StartIncreaseIfNotRunning()
        {
            if (_isIncreasing) return;
            _isIncreasing = true;
            _increaseCts?.Dispose();
            _increaseCts = new CancellationTokenSource();
            RunIncreaseAsync(_increaseCts.Token).Forget();
        }

        private async UniTaskVoid RunIncreaseAsync(CancellationToken token)
        {
            try
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    var max = _globalData.Get<SavablePlayerData>().Stars * 10;
                    var current = _globalData.Get<RuntimePlayerData>().CurrentWarmth;

                    if (current >= max) break;

                    _globalData.Edit<RuntimePlayerData>(data =>
                    {
                        var newVal = Mathf.Min(data.CurrentWarmth + _warmthIncreaseRate, max);
                        data.CurrentWarmth = newVal;
                    });
                    //Debug.Log("increase");
                    await UniTask.Delay(TimeSpan.FromSeconds(_increaseIntervalSeconds), cancellationToken: token);
                }
            }
            catch (OperationCanceledException) { }
            finally
            {
                _isIncreasing = false;
                if (_increaseCts != null)
                {
                    _increaseCts.Dispose();
                    _increaseCts = null;
                }
            }
        }

        public bool CheckWarmCost(int cost)
        {
            var current = _globalData.Get<RuntimePlayerData>().CurrentWarmth;
            return current >= cost;
        }
    }
}
