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
            if (Environment.UserName != "Nevsky") return;
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

            LocalizationManager.UploadToSheets(runtimeGraph);
            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }
    }
}