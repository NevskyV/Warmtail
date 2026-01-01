
using System;
using System.Collections.Generic;
using System.Linq;
using Data.Nodes;
using Systems;
using Unity.GraphToolkit.Editor;
using UnityEngine;

namespace Editor
{
    public abstract class DialogueNode : Node
    {
        public abstract void Setup(RuntimeNode node, Dictionary<INode, string> nodeIdMap);
    }
    
    [Serializable]
    public class StartNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort<int>("Dialogue Id").WithDefaultValue(0).Build();
            context.AddOutputPort("out").Build();
        }
    }
    
    [Serializable]
    public class TextNode : DialogueNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("in").Build();
            context.AddOutputPort("out").Build();

            context.AddInputPort<Character>("Character").Build();
            context.AddInputPort<string>("Override Name").Build();
            context.AddInputPort<CharacterEmotion>("Emotion").Build();
            context.AddInputPort<string>("Text").Build();
        }
        
        public override void Setup(RuntimeNode node, Dictionary<INode, string> nodeIdMap)
        {
            var textNode = (Data.Nodes.TextNode)node;
            var nextNode = GetOutputPortByName("out")?.firstConnectedPort;
            if(nextNode != null)
                node.NextNodeIds.Add(nodeIdMap[nextNode.GetNode()]);
            textNode.Character = NodePortHelper.GetPortValue<Character>(GetInputPortByName("Character"));
            textNode.Emotion = NodePortHelper.GetPortValue<CharacterEmotion>(GetInputPortByName("Emotion"));
            textNode.DisplayName = NodePortHelper.GetPortValue<string>(GetInputPortByName("Override Name"));
            textNode.Text = NodePortHelper.GetPortValue<string>(GetInputPortByName("Text"));
        }
    }
    
    [Serializable]
    public class ChoiceNode : DialogueNode
    {
        private const string _optionId = "Port Count";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("in").Build();

            var option = GetNodeOptionByName(_optionId);
            option.TryGetValue(out int portCount);
            for (int i = 0; i < portCount; i++)
            {
                context.AddInputPort<string>($"{i}").Build();
                context.AddOutputPort($"{i}").Build();
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>(_optionId).WithDefaultValue(2).Delayed();
        }
        
        public override void Setup(RuntimeNode node, Dictionary<INode, string> nodeIdMap)
        {
            var inputs = GetInputPorts().ToArray();
            var outputs = GetOutputPorts().ToArray();
            for (int i = 0; i < outputs.Length; i++)
            {
                ((Data.Nodes.ChoiceNode)node).Choices.Add(NodePortHelper.GetPortValue<string>(inputs[i+1]));
                var nextNode = outputs[i]?.firstConnectedPort;
                if (nextNode != null)
                {
                    node.NextNodeIds.Add(nodeIdMap[nextNode.GetNode()]);
                }
            }
        }
    }

    [Serializable]
    public class ConditionNode : DialogueNode
    {
        private const string PortCountOptionId = "PortCount";
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("in").Build();

            var option = GetNodeOptionByName(PortCountOptionId);
            option.TryGetValue(out int portCount);

            for (int i = 0; i < portCount; i++)
            {
                context.AddInputPort<ConditionStruct>($"{i}").WithDisplayName($"{i}").Build();
                context.AddOutputPort($"{i}").WithDisplayName($"{i}").Build();
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>(PortCountOptionId)
                .WithDefaultValue(2)
                .Delayed();
        }
        
        public override void Setup(RuntimeNode node, Dictionary<INode, string> nodeIdMap)
        {
            var inputs = GetInputPorts().ToArray();
            var outputs = GetOutputPorts().ToArray();
            for (int i = 0; i < outputs.Length; i++)
            {
                ((Data.Nodes.ConditionNode)node).Conditions.Add(NodePortHelper.GetPortValue<ConditionStruct>(inputs[i+1]));
              
                var nextNode = outputs[i]?.firstConnectedPort;
                if (nextNode != null)
                {
                    node.NextNodeIds.Add(nodeIdMap[nextNode.GetNode()]);
                }
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.CustomPropertyDrawer(typeof(SetStruct))]
        public class SetStructDrawer : UnityEditor.PropertyDrawer
        {
            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                position.height = UnityEditor.EditorGUIUtility.singleLineHeight;

                var varNameProp = property.FindPropertyRelative("VarName");
                var opProp = property.FindPropertyRelative("Operation");
                var valueProp = property.FindPropertyRelative("Value");

                float third = position.width / 3f;

                UnityEditor.EditorGUI.PropertyField(new Rect(position.x, position.y, third, position.height), varNameProp, GUIContent.none);
                UnityEditor.EditorGUI.PropertyField(new Rect(position.x + third, position.y, third, position.height), opProp, GUIContent.none);
                UnityEditor.EditorGUI.PropertyField(new Rect(position.x + 2*third, position.y, third, position.height), valueProp, GUIContent.none);
            }

            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
#endif
    }

    [Serializable]
    public class ActionNode : DialogueNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("in").Build();
            context.AddOutputPort("out").Build();
            
            context.AddInputPort<int>("Event Index").Build();
        }
        
        public override void Setup(RuntimeNode node, Dictionary<INode, string> nodeIdMap)
        {
            var nextNode = GetOutputPortByName("out")?.firstConnectedPort;
            if(nextNode != null)
                node.NextNodeIds.Add(nodeIdMap[nextNode.GetNode()]);
            ((Data.Nodes.ActionNode)node).EventInd = NodePortHelper.GetPortValue<int>(GetInputPortByName("Event Index"));
        }
    }
    
    [Serializable]
    public class SetNode : DialogueNode
    {
        private const string _optionId = "Port Count";
        
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("in").Build();

            var option = GetNodeOptionByName(_optionId);
            option.TryGetValue(out int portCount);
            for (int i = 0; i < portCount; i++)
            {
                context.AddInputPort<SetStruct>($"{i}").Build();
                context.AddOutputPort($"{i}").Build();
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<int>(_optionId).WithDefaultValue(2).Delayed();
        }
        
        public override void Setup(RuntimeNode node, Dictionary<INode, string> nodeIdMap)
        {
            var inputs = GetInputPorts().ToArray();
            var outputs = GetOutputPorts().ToArray();
            for (int i = 0; i < outputs.Length; i++)
            {
                ((Data.Nodes.SetNode)node).Variables.Add(NodePortHelper.GetPortValue<SetStruct>(inputs[i+1]));
              
                var nextNode = outputs[i]?.firstConnectedPort;
                if (nextNode != null)
                {
                    node.NextNodeIds.Add(nodeIdMap[nextNode.GetNode()]);
                }
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.CustomPropertyDrawer(typeof(ConditionStruct))]
        public class ConditionStructDrawer : UnityEditor.PropertyDrawer
        {
            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                position.height = UnityEditor.EditorGUIUtility.singleLineHeight;

                var varNameProp = property.FindPropertyRelative("VarName");
                var opProp = property.FindPropertyRelative("Operation");
                var valueProp = property.FindPropertyRelative("Value");

                float third = position.width / 3f;

                UnityEditor.EditorGUI.PropertyField(new Rect(position.x, position.y, third, position.height), varNameProp, GUIContent.none);
                UnityEditor.EditorGUI.PropertyField(new Rect(position.x + third, position.y, third, position.height), opProp, GUIContent.none);
                UnityEditor.EditorGUI.PropertyField(new Rect(position.x + 2*third, position.y, third, position.height), valueProp, GUIContent.none);
            }

            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                return UnityEditor.EditorGUIUtility.singleLineHeight;
            }
        }
#endif
    }
}