using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Nodes;
using Entities.Localization;
using Systems;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Editor
{
    [ScriptedImporter(1, DialogueGraph.AssetExtension)]
    public class DialogueGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);
            var runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();
            runtimeGraph.AllNodes = new();
            
            var nodeIdMap = new Dictionary<INode, string>();

            foreach (var node in editorGraph.GetNodes())
            {
                nodeIdMap[node] = Guid.NewGuid().ToString();
            }

            var startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
            runtimeGraph.DialogueId = NodePortHelper.GetPortValue<int>(startNode?.GetInputPortByName("Dialogue Id"));
            var entryPort = startNode?.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
            if(entryPort != null) runtimeGraph.EntryNodeId = nodeIdMap[entryPort.GetNode()];

            foreach (var iNode in editorGraph.GetNodes())
            {
                if (iNode is StartNode) continue;

                RuntimeNode runtimeNode = iNode switch
                {
                    TextNode _  => new Data.Nodes.TextNode {NodeId = nodeIdMap[iNode]},
                    ActionNode _  => new Data.Nodes.ActionNode{NodeId = nodeIdMap[iNode]},
                    ChoiceNode _  => new Data.Nodes.ChoiceNode {NodeId = nodeIdMap[iNode]},
                    ConditionNode _  => new Data.Nodes.ConditionNode {NodeId = nodeIdMap[iNode]},
                    SetNode _  => new Data.Nodes.SetNode {NodeId = nodeIdMap[iNode]},
                };
                ((DialogueNode)iNode).Setup(runtimeNode, nodeIdMap);
                runtimeGraph.AllNodes.Add(runtimeNode);
            }

            UploadToSheets(runtimeGraph);
            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }
        
        public void UploadToSheets(RuntimeDialogueGraph graph)
        {
            List<(string key, string ru)> mainLines = new();
            HashSet<Character> uniqueCharacters = new();

            foreach (var node in graph.AllNodes.OfType<Data.Nodes.TextNode>())
            {
                var sheet = node.Character.ToString();
                uniqueCharacters.Add(node.Character);
                var key = $"{sheet}_{graph.DialogueId}_{node.NodeId}";
                mainLines.Add((key, node.Text));
            }

            foreach (var uniqueCharacter in uniqueCharacters)
            {
                LocalizationExporter.Export(
                    uniqueCharacter.ToString(),
                    graph.DialogueId,
                    mainLines.Where(x => x.key.StartsWith(uniqueCharacter.ToString())));
            }

            List<(string key, string ru)> choicesLines = new();
            foreach (var node in graph.AllNodes.OfType<Data.Nodes.ChoiceNode>())
            {
                int i = 0;
                foreach (var line in node.Choices)
                {
                    var key = $"Player_{graph.DialogueId}_{node.NodeId}_{i}";
                    choicesLines.Add((key, line));
                    i++;
                }
            }
            LocalizationExporter.Export("Player",graph.DialogueId, choicesLines);
            LocalizationManager.LoadLocalizationTable();
        }
    }
}
