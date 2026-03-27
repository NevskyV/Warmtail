using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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
        public static bool FirstSpawn = true;

        private Dictionary<string, Vector2> _spawnPoints = new(){
            {"GameplayStart" , new Vector2(-7.66f, -37f)},
            {"GameplayNearHome" , new Vector2(9.31f, 18.91f)},
            {"HomeAtCarpet" , new Vector2(19.5f, -19.5f)},
            {"HomeNearDoor" , new Vector2(22.35f, -8.45f)},
            {"FearWorld" , new Vector2(-24.2f, 0f)}
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
            if (_globalData.Get<SavablePlayerData>().IsInFearWorld) pos = _spawnPoints["FearWorld"];

            else 
            {
                if (!isHomeOpened && !_atHome) pos = _spawnPoints["GameplayStart"];
                if (isHomeOpened && !_atHome) pos = _spawnPoints["GameplayNearHome"];
                if (_atHome && FirstSpawn) pos = _spawnPoints["HomeAtCarpet"];
                if (_atHome && !FirstSpawn) pos = _spawnPoints["HomeNearDoor"];
            }

            FirstSpawn = false;

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

        public void DieAtNearest(Vector2 from)
        {
            var nearest = _globalData.Get<SavablePlayerData>().RespawnPositions
                .Select(p => p.ToUnity())
                .OrderBy(p => Vector2.SqrMagnitude(p - from))
                .First();
            SetPosition(nearest);
        }

        private void SetPosition(Vector2 pos)
        {
            Vector2 bodyPos = _playerBody.position;
            _player.position = pos - (Vector2)_playerBody.position + (Vector2)_playerBody.parent.position;
        }
    }
}
