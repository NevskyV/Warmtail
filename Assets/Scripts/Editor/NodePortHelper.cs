using Unity.GraphToolkit.Editor;

namespace Systems
{
    public class NodePortHelper
    {
        public static T GetPortValue<T>(IPort port)
        {
            if (port == null) return default;

            if (port.isConnected)
            {
                if (port.firstConnectedPort.GetNode() is IVariableNode variableNode)
                {
                    variableNode.variable.TryGetDefaultValue(out T value);
                    return value;
                }
            }

            port.TryGetValue(out T fallBackValue);
            return fallBackValue;
        }
    }
}