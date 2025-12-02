using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Dungeon
{
    public class CombatUI : MonoBehaviour
    {
        [Header("Runtime References")]
        public CombatController CombatController;
        public Player Player;
        public Combatant Enemy;

        [Header("Root Panel")]
        [Tooltip("Parent panel that contains all combat UI. This will be toggled on/off.")]
        public GameObject CombatRootPanel;

        [Header("Player UI")]
        public TMP_Text PlayerNameText;
        public TMP_Text PlayerHpText;
        public Slider PlayerHpSlider;

        [Header("Enemy UI")]
        public TMP_Text EnemyNameText;
        public TMP_Text EnemyHpText;
        public Slider EnemyHpSlider;

        [Header("Log")]
        public TMP_Text CombatLogText;

        [Header("Controls")]
        public GameObject PlayerChoicePanel;
        public Button AttackButton;
        public Button UseItemButton;

        private void Start()
        {
            if (CombatRootPanel != null)
            {
                CombatRootPanel.SetActive(false);
            }
        }

        public void Init(CombatController controller, Player player, Combatant enemy)
        {
            CombatController = controller;
            Player = player;
            Enemy = enemy;

            if (CombatRootPanel != null)
            {
                CombatRootPanel.SetActive(true);
            }

            InitHud();
            ShowPlayerChoices(false);
        }

        private void Update()
        {
            if (Player == null || Enemy == null) return;
            UpdateHud();
        }

        private void InitHud()
        {
            if (Player != null)
            {
                if (PlayerNameText != null) PlayerNameText.text = Player.DisplayName;

                if (PlayerHpSlider != null)
                {
                    PlayerHpSlider.maxValue = Player.MaxHp;
                    PlayerHpSlider.value = Player.HP;
                }

                if (PlayerHpText != null)
                {
                    PlayerHpText.text = Player.HP + " / " + Player.MaxHp;
                }
            }

            if (Enemy != null)
            {
                if (EnemyNameText != null) EnemyNameText.text = Enemy.DisplayName;

                if (EnemyHpSlider != null)
                {
                    EnemyHpSlider.maxValue = Enemy.MaxHp;
                    EnemyHpSlider.value = Enemy.HP;
                }

                if (EnemyHpText != null)
                {
                    EnemyHpText.text = Enemy.HP + " / " + Enemy.MaxHp;
                }
            }
        }

        private void UpdateHud()
        {
            if (Player != null)
            {
                if (PlayerHpSlider != null) PlayerHpSlider.value = Player.HP;
                if (PlayerHpText != null) PlayerHpText.text = Player.HP + " / " + Player.MaxHp;
            }

            if (Enemy != null)
            {
                if (EnemyHpSlider != null) EnemyHpSlider.value = Enemy.HP;
                if (EnemyHpText != null) EnemyHpText.text = Enemy.HP + " / " + Enemy.MaxHp;
            }
        }
        public void UpdateLog(string message)
        {
            if (CombatLogText == null || string.IsNullOrEmpty(message)) return;

            if (string.IsNullOrEmpty(CombatLogText.text))
            {
                CombatLogText.text = message;
            }
            else
            {
                CombatLogText.text += "\n" + message;
            }

            string[] lines = CombatLogText.text.Split('\n');
            int maxLines = 3;

            if (lines.Length > maxLines)
            {
                int start = lines.Length - maxLines;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                int i = start;
                while (i < lines.Length)
                {
                    sb.Append(lines[i]);
                    if (i < lines.Length - 1)
                        sb.Append('\n');
                    i += 1;
                }
                CombatLogText.text = sb.ToString();
            }
        }

        public void ShowPlayerChoices(bool show)
        {
            if (PlayerChoicePanel != null)
            {
                PlayerChoicePanel.SetActive(show);
            }

            if (AttackButton != null)
            {
                AttackButton.interactable = show;
            }

            if (UseItemButton != null)
            {
                UseItemButton.interactable = show;
            }
        }

        public void HideCombatUI()
        {
            if (CombatRootPanel != null)
            {
                CombatRootPanel.SetActive(false);
            }
        }

        public void OnAttackClicked()
        {
            if (CombatController != null)
            {
                CombatController.PlayerChooseAttack();
            }
        }

        public void OnUseItemClicked()
        {
            if (CombatController != null)
            {
                CombatController.PlayerChooseUseItem();
            }
        }
    }
}
