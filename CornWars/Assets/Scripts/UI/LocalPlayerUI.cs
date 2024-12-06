using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LocalPlayerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _cornCapital;
        [SerializeField] private TMP_Text _currentRound;
        [SerializeField] private TMP_Text _currentLives;
        public int currentRound = 1;

        public void Awake()
        {
            _cornCapital.text = "Corn: 0";
            _currentRound.text = "Round: 1";
        }

        public void SubscribeToPlayerUpdates(Player localPlayer)
        {
            localPlayer.OnPowerChanged += UpdateCornUI;
        }

        private void UpdateCornUI(float currentPower)
        {
            _cornCapital.text = $"Corn: {currentPower}";
        }

        public void UpdateRoundUI()
        {
            _currentRound.text = $"Round: {currentRound}";
        }

        public void UpdateLivesUI(int lives)
        {
            _currentLives.text = $"{lives}";
        }
    }
}