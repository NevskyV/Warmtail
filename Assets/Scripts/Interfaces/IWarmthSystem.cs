namespace Interfaces
{
    public interface IWarmthSystem
    {
        bool CheckWarmCost(int cost);
        void DecreaseWarmth(int value);
    }
}

