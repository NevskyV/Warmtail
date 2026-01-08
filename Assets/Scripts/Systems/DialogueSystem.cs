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
        }
        
        public void StartDialogue(RuntimeDialogueGraph graph, ITextVisual visual, string id, IEventInvoker character = null)
        {
            if(graph.EntryNodeId == null) return;
            graph.AllNodes.ForEach(x => _nodeLookup.Add(x.NodeId, x));
            _dialogueGraph = graph;
            _currentNode = _nodeLookup[_dialogueGraph.EntryNodeId];
            
            _id = id;
            _visuals = visual;
            _visuals.ShowVisuals();
            Character = character;
            _prevActionMap = "Player";
            _input.SwitchCurrentActionMap("Dialogue");
            
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
            if (_visuals == null || _input == null) return;
            OnEndedDialogue?.Invoke(_id);
            _nodeLookup.Clear();
            Character = null;
            _dialogueGraph = null;
            _visuals.HideVisuals();
            _input.SwitchCurrentActionMap(_prevActionMap);
        }
    }
}