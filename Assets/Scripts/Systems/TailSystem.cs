using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Data.Player;
using Entities.PlayerScripts;
using Systems.Abilities;
using UnityEngine;
using Zenject;

namespace Systems
{
    public class TailSystem : IInitializable, IDisposable
    {
        public const int HeavyTailId = 1;
        public const int ColdTailId = 2;

        private readonly GlobalData _globalData;
        private readonly Rigidbody2D _playerRb;

        private HeavyTailAbility _heavyTail;
        private ColdTailAbility _coldTail;

        private int _activeTailId = -1;
        private CancellationTokenSource _cts;

        [Inject]
        public TailSystem(GlobalData globalData, Player player)
        {
            _globalData = globalData;
            _playerRb = player.Rigidbody;
        }

        public void Initialize()
        {
            _heavyTail = new HeavyTailAbility(_playerRb);
            _coldTail = new ColdTailAbility(_playerRb);

            _globalData.SubscribeTo<SavablePlayerData>(SyncFromData);
            SyncFromData();
        }

        private bool _disposed;

        private void SyncFromData()
        {
            if (_disposed) return;
            var id = _globalData.Get<SavablePlayerData>().ActiveTailId;
            if (id == _activeTailId) return;

            StopTickLoop();
            _activeTailId = id;

            if (id == HeavyTailId || id == ColdTailId)
            {
                _cts = new CancellationTokenSource();
                TickLoop(_cts.Token).Forget();
            }
        }

        private async UniTaskVoid TickLoop(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
                if (ct.IsCancellationRequested) break;

                switch (_activeTailId)
                {
                    case HeavyTailId:
                        _heavyTail.Tick();
                        break;
                    case ColdTailId:
                        _coldTail.Tick();
                        break;
                }
            }
        }

        private void StopTickLoop()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        public void Dispose()
        {
            _disposed = true;
            StopTickLoop();
        }
    }
}
