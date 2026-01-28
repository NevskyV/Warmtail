using System.Collections.Generic;
using Entities.Core;
using Entities.Props;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SequenceActions
{
    public class SavableStateAction : ISequenceAction
    {
        [SerializeField] private bool _active;
        [SerializeField] private List<string> _objectIds;
        [SerializeField] private bool _homeScene;
        
        public void Invoke()
        {
            if ((_homeScene && (SceneManager.GetActiveScene().name == "HomeIra" || SceneManager.GetActiveScene().name == "Home")) ||
                ( !_homeScene && (SceneManager.GetActiveScene().name == "Gameplay" || SceneManager.GetActiveScene().name == "GameplayIra") ) )
                {
                    _objectIds.ForEach(x => 
                        SavableObjectsResolver.FindObjectById<SavableStateObject>(x).ChangeState(_active));
                }
        }
    }
}