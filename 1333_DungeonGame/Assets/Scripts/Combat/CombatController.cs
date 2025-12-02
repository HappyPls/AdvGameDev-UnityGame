using System.Collections;
using UnityEngine;

namespace Dungeon
{
    public enum PlayerAction
    {
        None,
        Attack,
        UseItem
    }

    public class CombatController : MonoBehaviour
    {
        [Header("Combatants")]
        [SerializeField] private Player player;
        [SerializeField] private Combatant enemy;

        [Header("Dice Settings")]
        [SerializeField] private int diceCount = 5;

        [Header("Timing")]
        [SerializeField] private float turnDelay = 0.75f;

        private bool waitForPlayerChoice = false;
        private PlayerAction pendingAction = PlayerAction.None;

        private CombatState state = CombatState.None;
        private Coroutine combatRoutine;

        private CombatUI combatUI;

        private PlayerGridMover _mover;

        public Player Player => player;
        public Combatant Enemy => enemy;
        public void BeginCombat(Player p, Combatant e)
        {
            if (p == null || e == null)
            {
                Debug.LogWarning("CombatController.BeginCombat called with null player or enemy.");
                return;
            }

            player = p;
            enemy = e;

            if (combatUI == null)
            {
                combatUI = FindFirstObjectByType<CombatUI>();
            }

            if (combatUI != null)
            {
                combatUI.Init(this, player, enemy);
            }

            DisablePlayerMovement();

            if (combatRoutine != null)
            {
                StopCoroutine(combatRoutine);
            }

            combatRoutine = StartCoroutine(CombatLoop());
        }

        private IEnumerator CombatLoop()
        {
            state = CombatState.Intro;
            LogToUI("Combat started! " + player.DisplayName + " vs " + enemy.DisplayName);

            yield return new WaitForSeconds(turnDelay);

            while (state != CombatState.Victory && state != CombatState.Defeat)
            {
                state = CombatState.PlayerTurn;
                yield return PlayerTurn();

                if (enemy != null && enemy.IsDead)
                {
                    state = CombatState.Victory;
                    break;
                }

                yield return new WaitForSeconds(turnDelay);

                state = CombatState.EnemyTurn;
                yield return EnemyTurn();

                if (player != null && player.IsDead)
                {
                    state = CombatState.Defeat;
                    break;
                }

                yield return new WaitForSeconds(turnDelay);
            }

            if (state == CombatState.Victory)
            {
                LogToUI("Player wins!");
            }
            else if (state == CombatState.Defeat)
            {
                GameOverUI go = FindFirstObjectByType<GameOverUI>();
                if (go != null)
                {
                    go.ShowGameOver();
                }
                LogToUI("Player was defeated...");
            }

            state = CombatState.Finished;
            FinishCombat();
        }

        private IEnumerator PlayerTurn()
        {
            LogToUI("PLAYER TURN");

            pendingAction = PlayerAction.None;
            waitForPlayerChoice = true;

            if (combatUI != null)
            {
                combatUI.ShowPlayerChoices(true);
            }

            while (pendingAction == PlayerAction.None)
            {
                yield return null;
            }

            waitForPlayerChoice = false;

            if (combatUI != null)
            {
                combatUI.ShowPlayerChoices(false);
            }

            if (pendingAction == PlayerAction.Attack)
            {
                yield return DiceAttack(player, enemy, "Player");
            }
            else if (pendingAction == PlayerAction.UseItem)
            {
                LogToUI("Player used an item. Turn is consumed.");
            }
        }

        private IEnumerator EnemyTurn()
        {
            LogToUI("ENEMY TURN");
            yield return DiceAttack(enemy, player, "Enemy");
        }

        private IEnumerator DiceAttack(Combatant attacker, Combatant defender, string label)
        {
            if (attacker == null || defender == null) yield break;

            state = CombatState.Resolving;

            DiceHandResult hand = DicePokerHelper.RollAndEvaluate(diceCount);

            int baseAtk = attacker.GetTotalAttackPower();
            int rawDamage = Mathf.RoundToInt(baseAtk * hand.DamageMultiplier);

            int defense = defender.GetTotalDefense();
            int finalDamage = rawDamage - defense;
            if (finalDamage < 0) finalDamage = 0;

            defender.TakeDamage(finalDamage);

            string diceText = DiceToString(hand.DiceValues);
            string msg = label + " rolled [" + diceText + "] -> " +
                         hand.HandType + " (x" + hand.DamageMultiplier + ")" +
                         " -> " + finalDamage + " dmg";

            LogToUI(msg);

            yield return null;
        }
        private void LogToUI(string message)
        {
            Debug.Log(message);
            if (combatUI != null)
            {
                combatUI.UpdateLog(message);
            }
        }

        private string DiceToString(int[] dice)
        {
            if (dice == null || dice.Length == 0) return "";
            string s = "";
            int i = 0;
            while (i < dice.Length)
            {
                s += dice[i];
                if (i < dice.Length - 1) s += ", ";
                i += 1;
            }
            return s;
        }

        private void DisablePlayerMovement()
        {
            if (player == null) return;

            if (_mover == null)
            {
                _mover = player.GetComponent<PlayerGridMover>();
            }

            if (_mover != null)
            {
                _mover.enabled = false;
            }
        }

        private void EnablePlayerMovement()
        {
            if (_mover != null)
            {
                _mover.enabled = true;
            }
        }

        private void FinishCombat()
        {
            EnablePlayerMovement();

            if (combatUI != null)
            {
                combatUI.HideCombatUI();
            }

            enemy = null;
            state = CombatState.None;
            pendingAction = PlayerAction.None;
            waitForPlayerChoice = false;
        }

        public void PlayerChooseAttack()
        {
            if (state == CombatState.PlayerTurn &&
                waitForPlayerChoice &&
                pendingAction == PlayerAction.None)
            {
                pendingAction = PlayerAction.Attack;
            }
        }

        public void PlayerChooseUseItem()
        {
            if (state == CombatState.PlayerTurn &&
                waitForPlayerChoice &&
                pendingAction == PlayerAction.None)
            {
                pendingAction = PlayerAction.UseItem;
            }
        }
    }
}
