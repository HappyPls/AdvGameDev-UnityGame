using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Dungeon
{
    public class RoomBase : MonoBehaviour
    {
        [SerializeField] private GameObject NorthDoorway, EastDoorway, SouthDoorway, WestDoorway;

        private RoomBase _north, _south, _east, _west;

        public void SetRooms(RoomBase roomNorth, RoomBase roomEast, RoomBase roomSouth, RoomBase roomWest)
        {
            _north = roomNorth;
            NorthDoorway.SetActive(_north == null);
            _south = roomSouth;
            SouthDoorway.SetActive(_south == null);
            _east = roomEast;
            EastDoorway.SetActive(_east == null);
            _west = roomWest;
            WestDoorway.SetActive(_west == null);
        }
    }
}