using UnityEngine;
using UnityEngine.InputSystem;

namespace Dungeon
{
    public class PlayerGridMover : MonoBehaviour
    {
        public DungeonController Controller;
        public float MoveCooldown = 0.15f;

        private float _cooldownTimer;
        void Start()
        {
            if (Controller == null)
            {
                Controller = FindFirstObjectByType<DungeonController>();
            }
        }

        void Update()
        {
            if (Controller == null) return;

            if (_cooldownTimer > 0f)
            {
                _cooldownTimer -= Time.deltaTime;
                return;
            }

            Keyboard kb = Keyboard.current;
            if (kb == null) return;
            
            bool moved = false;

            if ((kb.wKey != null && kb.wKey.wasPressedThisFrame) ||
                (kb.upArrowKey != null && kb.upArrowKey.wasPressedThisFrame))
            {
                moved = Controller.TryMove(Direction.North);
            }
            else if ((kb.sKey != null && kb.sKey.wasPressedThisFrame) ||
                     (kb.downArrowKey != null && kb.downArrowKey.wasPressedThisFrame))
            {
                moved = Controller.TryMove(Direction.South);
            }
            else if ((kb.dKey != null && kb.dKey.wasPressedThisFrame) ||
                     (kb.rightArrowKey != null && kb.rightArrowKey.wasPressedThisFrame))
            {
                moved = Controller.TryMove(Direction.East);
            }
            else if ((kb.aKey != null && kb.aKey.wasPressedThisFrame) ||
                     (kb.leftArrowKey != null && kb.leftArrowKey.wasPressedThisFrame))
            {
                moved = Controller.TryMove(Direction.West);
            }

            if (moved)
            {
                _cooldownTimer = MoveCooldown;
            }
        }
    }

}
