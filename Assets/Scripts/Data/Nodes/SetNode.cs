using System;
using System.Collections.Generic;
using System.Globalization;
using Systems;
using UnityEngine;
using Zenject;

namespace Data.Nodes
{
    public record SetNode : RuntimeNode
    {
        [field: SerializeField] public List<SetStruct> Variables { get; private set; } = new();

        [Inject] private GlobalData _globalData;
        [Inject] private DialogueSystem _dialogueSystem;

        public override void Activate()
        {
            var varDataList = _globalData.Get<DialogueVarData>().Variables;
            foreach (var setStruct in Variables)
            {
                var varData = varDataList.Find(x => x.Name == setStruct.VarName);
                var index = varDataList.IndexOf(varData);
                switch (setStruct.Operation)
                {
                    case MathOperation.Assign:
                        varData.Value = setStruct.Value;
                        break;
                    case MathOperation.Add:
                        switch (varData.Type)
                        {
                            case DialogueVar.VarType.Int:
                                varData.Value = (int.Parse(varData.Value) + int.Parse(setStruct.Value)).ToString();
                                break;
                            case DialogueVar.VarType.Float:
                                varData.Value = (float.Parse(varData.Value) + float.Parse(setStruct.Value)).ToString(CultureInfo.InvariantCulture);
                                break;
                            case DialogueVar.VarType.String:
                                varData.Value += setStruct.Value;
                                break;
                        }
                        break;
                    case MathOperation.Subtract:
                        switch (varData.Type)
                        {
                            case DialogueVar.VarType.Int:
                                varData.Value = (int.Parse(varData.Value) - int.Parse(setStruct.Value)).ToString();
                                break;
                            case DialogueVar.VarType.Float:
                                varData.Value = (float.Parse(varData.Value) - float.Parse(setStruct.Value)).ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                        break;
                    case MathOperation.Multiply:
                        switch (varData.Type)
                        {
                            case DialogueVar.VarType.Int:
                                varData.Value = (int.Parse(varData.Value) * int.Parse(setStruct.Value)).ToString();
                                break;
                            case DialogueVar.VarType.Float:
                                varData.Value = (float.Parse(varData.Value) * float.Parse(setStruct.Value)).ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                        break;
                    case MathOperation.Divide:
                        switch (varData.Type)
                        {
                            case DialogueVar.VarType.Int:
                                varData.Value = (int.Parse(varData.Value) / int.Parse(setStruct.Value)).ToString();
                                break;
                            case DialogueVar.VarType.Float:
                                varData.Value = (float.Parse(varData.Value) / float.Parse(setStruct.Value)).ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                        break;
                }

                _globalData.Edit<DialogueVarData>(x => x.Variables[index] = varData);
            }
        
            _dialogueSystem.SetNewNode();
            _dialogueSystem.ActivateNewNode();
        }
    }

    [Serializable]
    public class SetStruct
    {
        public string VarName;
        public MathOperation Operation;
        public string Value;
    }
    
    [Serializable]
    public enum MathOperation {Assign, Add, Subtract, Multiply, Divide}
}