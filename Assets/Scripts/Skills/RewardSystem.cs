using UnityEngine;
using System;

namespace RPS.Skills
{
    /// <summary>
    /// 보상 지급 시스템 Skill
    /// </summary>
    public class RewardSystem : MonoBehaviour
    {
        [Header("Currency Settings")]
        [SerializeField] private int _startingGold = 100;
        [SerializeField] private int _startingGems = 0;

        [Header("Current Currency")]
        [SerializeField] private int _currentGold = 0;
        [SerializeField] private int _currentGems = 0;

        // 이벤트
        public event Action<int, int> OnCurrencyChanged; // gold, gems

        /// <summary>
        /// 현재 골드
        /// </summary>
        public int CurrentGold => _currentGold;

        /// <summary>
        /// 현재 젬
        /// </summary>
        public int CurrentGems => _currentGems;

        private void Awake()
        {
            _currentGold = _startingGold;
            _currentGems = _startingGems;

            Debug.Log($"[RewardSystem] Initialized - Gold: {_currentGold}, Gems: {_currentGems}");
        }

        /// <summary>
        /// 골드 추가
        /// </summary>
        public void AddGold(int amount)
        {
            if (amount <= 0) return;

            _currentGold += amount;
            Debug.Log($"[RewardSystem] Gold added: +{amount} (Total: {_currentGold})");
            OnCurrencyChanged?.Invoke(_currentGold, _currentGems);
        }

        /// <summary>
        /// 골드 차감
        /// </summary>
        public bool SpendGold(int amount)
        {
            if (_currentGold < amount)
            {
                Debug.LogWarning($"[RewardSystem] Not enough gold! Have {_currentGold}, need {amount}");
                return false;
            }

            _currentGold -= amount;
            Debug.Log($"[RewardSystem] Gold spent: -{amount} (Total: {_currentGold})");
            OnCurrencyChanged?.Invoke(_currentGold, _currentGems);
            return true;
        }

        /// <summary>
        /// 젬 추가
        /// </summary>
        public void AddGems(int amount)
        {
            if (amount <= 0) return;

            _currentGems += amount;
            Debug.Log($"[RewardSystem] Gems added: +{amount} (Total: {_currentGems})");
            OnCurrencyChanged?.Invoke(_currentGold, _currentGems);
        }

        /// <summary>
        /// 젬 차감
        /// </summary>
        public bool SpendGems(int amount)
        {
            if (_currentGems < amount)
            {
                Debug.LogWarning($"[RewardSystem] Not enough gems! Have {_currentGems}, need {amount}");
                return false;
            }

            _currentGems -= amount;
            Debug.Log($"[RewardSystem] Gems spent: -{amount} (Total: {_currentGems})");
            OnCurrencyChanged?.Invoke(_currentGold, _currentGems);
            return true;
        }

        /// <summary>
        /// 매치 보상 지급
        /// </summary>
        /// <param name="result">1=승리, 0=무승부, -1=패배</param>
        /// <param name="streakBonus">연승 보너스</param>
        public void GrantMatchReward(int result, int streakBonus)
        {
            if (result > 0)
            {
                // 승리 보상: 기본 10 + 연승 보너스
                int totalGold = 10 + streakBonus;
                AddGold(totalGold);
                Debug.Log($"[RewardSystem] Victory reward: {totalGold} gold (base: 10, streak: {streakBonus})");
            }
            // 패배/무승부 시 보상 없음
        }

        /// <summary>
        /// 일일 출석 보상
        /// </summary>
        public void GrantDailyReward(int day)
        {
            // 1일: 10, 2일: 20, ..., 7일: 100, 그 후: 50
            int reward = Mathf.Min(day * 10, 100);
            if (day > 7) reward = 50;

            AddGold(reward);
            Debug.Log($"[RewardSystem] Daily reward (Day {day}): {reward} gold");
        }

        /// <summary>
        /// 광고 시청 보상
        /// </summary>
        public void GrantAdReward()
        {
            int reward = 50;
            AddGold(reward);
            Debug.Log($"[RewardSystem] Ad reward: {reward} gold");
        }

        /// <summary>
        /// 골드가 충분한지 확인
        /// </summary>
        public bool HasEnoughGold(int amount)
        {
            return _currentGold >= amount;
        }

        /// <summary>
        /// 젬이 충분한지 확인
        /// </summary>
        public bool HasEnoughGems(int amount)
        {
            return _currentGems >= amount;
        }
    }
}
