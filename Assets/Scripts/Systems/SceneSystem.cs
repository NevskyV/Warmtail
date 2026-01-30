using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Zenject;
using Entities.PlayerScripts;
using Data;
using Entities.Core;
using Data.Player;
using System;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;

namespace Systems
{
    public class SceneSystem 
    {
        private Transform _player;
        private Transform _playerBody;
        private GlobalData _globalData;
        private bool _atHome;
        public static bool _firstSpawn = true;

        private Dictionary<string, Vector2> spawnPoints = new(){
            {"GameplayStart" , new Vector2(-25.5f, 1)},
            {"GameplayNearHome" , new Vector2(124.2f, 259f)},
            {"HomeAtCarpet" , new Vector2(18.15f, -24.82f)},
            {"HomeNearDoor" , new Vector2(22.55f, 3.08f)},
        };

        [Inject]        
        private void Construct(Player player, GlobalData globalData, SceneLoader sceneLoader)
        {
            _player = player.transform;
            _playerBody = player.Rigidbody.transform;
            _globalData = globalData;
        }

        public void Spawn()
        {
            if ((SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "HomeIra" ) 
                && !_globalData.Get<SavablePlayerData>().IsHomeOpened)
                _globalData.Edit<SavablePlayerData>(data => data.IsHomeOpened = true);

            _atHome = (SceneManager.GetActiveScene().name == "Home" || SceneManager.GetActiveScene().name == "HomeIra");
            bool isHomeOpened = _globalData.Get<SavablePlayerData>().IsHomeOpened;
            
            Vector2 pos = new();
            if (!isHomeOpened && !_atHome) pos = spawnPoints["GameplayStart"];
            if (isHomeOpened && !_atHome) pos = spawnPoints["GameplayNearHome"];
            if (_atHome && _firstSpawn) pos = spawnPoints["HomeAtCarpet"];
            if (_atHome && !_firstSpawn) pos = spawnPoints["HomeNearDoor"];

            _firstSpawn = false;

            SetPosition(pos);
        }

        public void Die()
        {
            var pos = new List<Vector2>();
            var systemPos = _globalData.Get<SavablePlayerData>().RespawnPositions;

            foreach (var p in systemPos) {
                pos.Add(p.ToUnity());
            }

            SetPosition(pos.GetRandom());
        }

        private void SetPosition(Vector2 pos)
        {
            Vector2 bodyPos = _playerBody.position;
            _player.position = pos - (Vector2)_playerBody.position + (Vector2)_playerBody.parent.position;
        }
    }
}
