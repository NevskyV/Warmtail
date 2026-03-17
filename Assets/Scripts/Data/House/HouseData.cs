
using System.Collections.Generic;
using System;
using TriInspector;
using UnityEngine;

namespace Data.House
{
    [Serializable]
    public class HouseData : ISavableData
    {
        [SerializeField, TableList(Draggable = true, AlwaysExpanded = true)]
        public List<PairForHouseItem> PlacedHouseItems = new();
        public int HouseTrack;
    }
    [Serializable]
    public class PairForHouseItem
    {
        public int HouseItemDataId;
        public float PositionX;
        public float PositionY;
        public PairForHouseItem(int houseItemDataId, float positionX, float positionY)
        {
            HouseItemDataId = houseItemDataId;
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}