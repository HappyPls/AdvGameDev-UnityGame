using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.CompilerServices;

[CreateAssetMenu(fileName = "GridSettings", menuName = "Scriptable Objects/GridSettings")]
public class GridSettings : ScriptableObject
{
    [SerializeField] private int _rows = 10;
    [SerializeField] private int _cols = 10;
    [SerializeField] private float _roomSize = 1f;
    [SerializeField] private bool _useXZPlane = true;

    public int Rows => _rows;
    public int Cols => _cols;
    public float RoomSize => _roomSize;
    public bool UseXZPlane => _useXZPlane;

    public int MinimumTreasureRooms;
    public int MinimumCombatRooms;
    public int BossRooms;
}
