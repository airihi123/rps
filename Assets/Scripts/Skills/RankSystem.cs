using UnityEngine;
using System;

namespace RPS.Skills
{
    /// <summary>
    /// 계급 enum
    /// </summary>
    public enum RankTier
    {
        Bronze,      // 0-499
        Silver,      // 500-999
        Gold,        // 1000-1499
        Platinum,    // 1500-1999
        Diamond,     // 2000-2499
        Master,      // 2500-2999
        GrandMaster  // 3000+
    }

    /// <summary>
    /// 디비전 enum
    /// </summary>
    public enum Division
    {
        IV,  // 하위
        III,
        II,
        I    // 상위
    }

    /// <summary>
    /// 랭킹 및 계급 시스템 Skill
    /// </summary>
    [Serializable]
    public class RankSystem
    {
        [Header("Rank Settings")]
        [SerializeField] private int _baseWinPoints = 20;
        [SerializeField] private int _baseLosePoints = -15;
        [SerializeField] private int _drawPoints = 5;

        [Header("Current Rank State")]
        [SerializeField] private int _currentPoints = 0;
        [SerializeField] private int _currentStreak = 0;

        // 이벤트
        public event Action<int> OnPointsChanged;
        public event Action<RankTier, Division> OnRankChanged;

        /// <summary>
        /// 현재 점수
        /// </summary>
        public int CurrentPoints => _currentPoints;

        /// <summary>
        /// 현재 티어
        /// </summary>
        public RankTier CurrentTier => GetTierFromPoints(_currentPoints);

        /// <summary>
        /// 현재 디비전
        /// </summary>
        public Division CurrentDivision => GetDivisionFromPoints(_currentPoints);

        /// <summary>
        /// 현재 티어/디비전 문자열 표현
        /// </summary>
        public string CurrentRankDisplay => $"{GetCurrentTierName()} {GetDivisionSymbol()}";

        public RankSystem()
        {
            Debug.Log("[RankSystem] Initialized");
        }

        /// <summary>
        /// 매치 결과에 따른 점수 계산
        /// </summary>
        /// <param name="result">1=승리, 0=무승부, -1=패배</param>
        /// <param name="streakMultiplier">연승 배수</param>
        /// <returns>획득/손실 점수</returns>
        public int CalculatePoints(int result, int streakMultiplier)
        {
            int points = 0;

            if (result > 0)
            {
                // 승리: 기본 점수 * 연승 배수
                points = _baseWinPoints * streakMultiplier;
            }
            else if (result < 0)
            {
                // 패배: 기본 감소 점수
                points = _baseLosePoints;
            }
            else
            {
                // 무승부: 소량 획득
                points = _drawPoints;
            }

            return points;
        }

        /// <summary>
        /// 점수 추가 및 랭크 업데이트
        /// </summary>
        public void AddPoints(int points)
        {
            RankTier oldTier = CurrentTier;
            _currentPoints = Mathf.Max(0, _currentPoints + points);

            Debug.Log($"[RankSystem] Points changed: {points} (Total: {_currentPoints})");
            OnPointsChanged?.Invoke(_currentPoints);

            // 랭크 변경 확인
            RankTier newTier = CurrentTier;
            if (oldTier != newTier)
            {
                Debug.Log($"[RankSystem] Rank changed! {oldTier} -> {newTier}");
                OnRankChanged?.Invoke(newTier, CurrentDivision);
            }
        }

        /// <summary>
        /// 점수로 티어 계산
        /// </summary>
        private RankTier GetTierFromPoints(int points)
        {
            if (points >= 3000) return RankTier.GrandMaster;
            if (points >= 2500) return RankTier.Master;
            if (points >= 2000) return RankTier.Diamond;
            if (points >= 1500) return RankTier.Platinum;
            if (points >= 1000) return RankTier.Gold;
            if (points >= 500) return RankTier.Silver;
            return RankTier.Bronze;
        }

        /// <summary>
        /// 점수로 디비전 계산
        /// </summary>
        private Division GetDivisionFromPoints(int points)
        {
            int tierPoints = points % 500;
            int division = tierPoints / 125; // 0-3

            return (Division)Mathf.Clamp(3 - division, 0, 3);
        }

        /// <summary>
        /// 현재 티어 이름
        /// </summary>
        private string GetCurrentTierName()
        {
            return CurrentTier.ToString();
        }

        /// <summary>
        /// 디비전 기호
        /// </summary>
        private string GetDivisionSymbol()
        {
            return CurrentDivision.ToString();
        }

        /// <summary>
        /// 다음 티어까지 남은 점수
        /// </summary>
        public int GetPointsToNextTier()
        {
            RankTier tier = CurrentTier;

            return tier switch
            {
                RankTier.Bronze => 500 - _currentPoints,
                RankTier.Silver => 1000 - _currentPoints,
                RankTier.Gold => 1500 - _currentPoints,
                RankTier.Platinum => 2000 - _currentPoints,
                RankTier.Diamond => 2500 - _currentPoints,
                RankTier.Master => 3000 - _currentPoints,
                RankTier.GrandMaster => 0, // 최고 티어
                _ => 0
            };
        }
    }
}
