using System.Collections.Generic;
using System;
using Data;
using Data.Nodes;
using Interfaces;
using UnityEngine.InputSystem;
using Zenject;
using UnityEngine;

namespace Systems
{
    
    public class DialogueSystem
    {
        private RuntimeDialogueGraph _dialogueGraph;
        private DiContainer _diContainer;
        private ITextVisual _visuals;
        private PlayerInput _input;
        private string _prevActionMap;
        private Dictionary<string, RuntimeNode> _nodeLookup = new();
        private RuntimeNode _currentNode;
        
        public ITextVisual Visuals => _visuals;
        
        public RuntimeDialogueGraph DialogueGraph => _dialogueGraph;
        public IEventInvoker Character { get; private set; }

        private string _id;
        public static Action<string> OnEndedDialogue = delegate{};

        [Inject]
        private void Construct(DiContainer container, PlayerInput input)
        {
            _diContainer = container;
            _input = input;
            Debug.Log("Ira 0 " + input);
        }
        
        public void StartDialogue(RuntimeDialogueGraph graph, ITextVisual visual, string id, IEventInvoker character = null)
        {
            if (graph == null)
            {
                Debug.LogError("DialogueSystem: Graph is null!");
                return;
            }
            
            if (graph.EntryNodeId == null)
            {
                Debug.LogError($"DialogueSystem: EntryNodeId is null for dialogue '{id}'");
                return;
            }
            
            if (graph.AllNodes == null || graph.AllNodes.Count == 0)
            {
                Debug.LogError($"DialogueSystem: AllNodes is null or empty for dialogue '{id}'");
                return;
            }
            
            _nodeLookup.Clear(); // Очистить перед заполнением
            graph.AllNodes.ForEach(x => _nodeLookup.Add(x.NodeId, x));
            
            _dialogueGraph = graph;
            _currentNode = _nodeLookup[_dialogueGraph.EntryNodeId];
            
            _id = id;
            _visuals = visual;
            _visuals?.ShowVisuals();
            Character = character;
            _prevActionMap = "Player";
            
            if (_input != null)
            {
                _input.SwitchCurrentActionMap("Dialogue");
                Debug.Log("Ira 1 " + _input);
            }
            
            ActivateNewNode();
        }

        public void SetNewNode(int portIndex = 0)
        {
            if (portIndex < _currentNode.NextNodeIds.Count)
            {
                var nextNode = _currentNode.NextNodeIds[portIndex];
                _currentNode = _nodeLookup[nextNode];
            }
            else _currentNode = null;
        }

        public void ActivateNewNode()
        {
            Debug.Log("Ira 2");
            if (_dialogueGraph == null || _currentNode == null)
            {
                EndDialogue();
                return;
            }
            _diContainer.Inject(_currentNode);
            _currentNode.Activate();
        }
        
        private void EndDialogue()
        {
            Debug.Log("Ira 3");
            if (_visuals == null || _input == null) return;
            Debug.Log("Ira 4");
            OnEndedDialogue?.Invoke(_id);
            _nodeLookup.Clear();
            Character = null;
            _dialogueGraph = null;
            _visuals.HideVisuals();
            _input.SwitchCurrentActionMap(_prevActionMap);
        }
    }
}