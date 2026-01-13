using UnityEngine;
using System;

namespace RPS.Skills
{
    /// <summary>
    /// 연승 보너스 시스템 Skill
    /// </summary>
    public class StreakSystem : MonoBehaviour
    {
        [Header("Streak Settings")]
        [SerializeField] private int _maxStreakMultiplier = 4;
        [SerializeField] private int[] _goldReward = { 10, 20, 30, 40 }; // 1연승, 2연승, 3연승, 4연승+

        [Header("Current Streak State")]
        [SerializeField] private int _currentStreak = 0;

        // 이벤트
        public event Action<int> OnStreakChanged;
        public event Action<int, int> OnStreakReward; // streak, goldAmount

        /// <summary>
        /// 현재 연승 횟수
        /// </summary>
        public int CurrentStreak => _currentStreak;

        /// <summary>
        /// 현재 연승 배수
        /// </summary>
        public int CurrentMultiplier => Mathf.Min(_currentStreak, _maxStreakMultiplier);

        private void Awake()
        {
            Debug.Log("[StreakSystem] Initialized");
        }

        /// <summary>
        /// 매치 승리 시 연승 증가
        /// </summary>
        public void AddWin()
        {
            _currentStreak++;
            int multiplier = CurrentMultiplier;
            int goldReward = GetGoldReward();

            Debug.Log($"[StreakSystem] Streak increased to {_currentStreak} (Multiplier: x{multiplier}, Gold: +{goldReward})");
            OnStreakChanged?.Invoke(_currentStreak);
            OnStreakReward?.Invoke(_currentStreak, goldReward);
        }

        /// <summary>
        /// 매치 패배 시 연승 리셋
        /// </summary>
        public void ResetStreak()
        {
            Debug.Log($"[StreakSystem] Streak reset (was {_currentStreak})");
            _currentStreak = 0;
            OnStreakChanged?.Invoke(_currentStreak);
        }

        /// <summary>
        /// 연승 방어 (광고 시청 등으로 배수 유지)
        /// </summary>
        public void DefendStreak()
        {
            Debug.Log($"[StreakSystem] Streak defended! Current streak: {_currentStreak}");
        }

        /// <summary>
        /// 현재 연승에 따른 골드 보상 계산
        /// </summary>
        public int GetGoldReward()
        {
            if (_currentStreak == 0) return 0;

            int index = Mathf.Min(_currentStreak - 1, _goldReward.Length - 1);
            return _goldReward[index];
        }

        /// <summary>
        /// 다음 연승 달성 시 예상 보상
        /// </summary>
        public int GetNextReward()
        {
            int nextStreak = _currentStreak + 1;
            int index = Mathf.Min(nextStreak - 1, _goldReward.Length - 1);
            return _goldReward[index];
        }
    }
}
