using System.Collections.Generic;
using Entities.Core;
using Entities.Probs;
using Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.SequenceActions
{
    public class SavableStateAction : ISequenceAction
    {
        [SerializeField] private bool _active;
        [SerializeField] private List<string> _objectIds;
        [SerializeField] private bool _anotherScene;
        
        public void Invoke()
        {
            if (!_anotherScene || SceneManager.GetActiveScene().name == "Gameplay" || SceneManager.GetActiveScene().name == "GameplayIra"){
                Debug.Log("Ira invoke");
                 _objectIds.ForEach(x => 
                    SavableObjectsResolver.FindObjectById<SavableStateObject>(x).ChangeState(_active));}
        }
    }
}