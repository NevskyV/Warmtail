using Data;
using Data.Player;
using UnityEngine;
using Zenject;

public class ShellReward : MonoBehaviour
{
    private GlobalData _globalData;

    [Inject]
    public void Construct(GlobalData globalData)
    {
        _globalData = globalData;
    }

    public void Reward()
    {
        _globalData.Edit<SavablePlayerData>(player =>
        {
            player.Shells += 50;
        });
    }
}