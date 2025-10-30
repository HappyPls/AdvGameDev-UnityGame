
using UnityEngine;

public class EncounterRoomView : MonoBehaviour
{
    [SerializeField] private GameObject monsterMarker;

    public void SetCleared(bool cleared)
    {
        if (monsterMarker) monsterMarker.SetActive(!cleared);
    }
}