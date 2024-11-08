using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LocalPlayerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _currentPowerAvailable;

        public void Awake()
        {
            _currentPowerAvailable.text = "Corn: 0";
        }

        public void SubscribeToPlayerUpdates(Player localPlayer)
        {
            localPlayer.OnPowerChanged += UpdateUI;
        }

        private void UpdateUI(float currentPower)
        {
            _currentPowerAvailable.text = $"Corn: {currentPower}";
        }
    }
}