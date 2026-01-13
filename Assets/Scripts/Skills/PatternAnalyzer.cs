using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RPS.Skills
{
    /// <summary>
    /// 상대 패턴 분석 Skill - 심리전 핵심 기능
    /// </summary>
    public class PatternAnalyzer : MonoBehaviour
    {
        [Header("Analysis Settings")]
        [SerializeField] private int _historySize = 10; // 최근 10수 기록

        [Header("Pattern Data")]
        [SerializeField] private List<RpsMove> _moveHistory = new List<RpsMove>();
        [SerializeField] private int _rockCount = 0;
        [SerializeField] private int _paperCount = 0;
        [SerializeField] private int _scissorsCount = 0;

        /// <summary>
        /// 전체 기록 수
        /// </summary>
        public int TotalMoves => _moveHistory.Count;

        /// <summary>
        /// 바위 낼 확률 (%)
        /// </summary>
        public float RockPercentage => TotalMoves > 0 ? (float)_rockCount / TotalMoves * 100f : 33.3f;

        /// <summary>
        /// 보 낼 확률 (%)
        /// </summary>
        public float PaperPercentage => TotalMoves > 0 ? (float)_paperCount / TotalMoves * 100f : 33.3f;

        /// <summary>
        /// 가위 낼 확률 (%)
        /// </summary>
        public float ScissorsPercentage => TotalMoves > 0 ? (float)_scissorsCount / TotalMoves * 100f : 33.3f;

        private void Awake()
        {
            Debug.Log("[PatternAnalyzer] Initialized");
        }

        /// <summary>
        /// 상대의 수 기록
        /// </summary>
        public void RecordMove(RpsMove move)
        {
            if (move == RpsMove.None) return;

            // 기록 추가
            _moveHistory.Add(move);

            // 카운트 업데이트
            switch (move)
            {
                case RpsMove.Rock:
                    _rockCount++;
                    break;
                case RpsMove.Paper:
                    _paperCount++;
                    break;
                case RpsMove.Scissors:
                    _scissorsCount++;
                    break;
            }

            // 기록 제한 (오래된 기록 삭제)
            if (_moveHistory.Count > _historySize)
            {
                RpsMove oldMove = _moveHistory[0];
                _moveHistory.RemoveAt(0);

                switch (oldMove)
                {
                    case RpsMove.Rock:
                        _rockCount--;
                        break;
                    case RpsMove.Paper:
                        _paperCount--;
                        break;
                    case RpsMove.Scissors:
                        _scissorsCount--;
                        break;
                }
            }

            Debug.Log($"[PatternAnalyzer] Move recorded: {move} (R:{RockPercentage:F1}% P:{PaperPercentage:F1}% S:{ScissorsPercentage:F1}%)");
        }

        /// <summary>
        /// 최근 N수 가져오기
        /// </summary>
        public List<RpsMove> GetRecentMoves(int count)
        {
            int startIndex = Mathf.Max(0, _moveHistory.Count - count);
            return _moveHistory.Skip(startIndex).ToList();
        }

        /// <summary>
        /// 가장 자주 내는 손 가져오기
        /// </summary>
        public RpsMove GetMostFrequentMove()
        {
            if (_rockCount >= _paperCount && _rockCount >= _scissorsCount)
                return RpsMove.Rock;
            if (_paperCount >= _rockCount && _paperCount >= _scissorsCount)
                return RpsMove.Paper;
            return RpsMove.Scissors;
        }

        /// <summary>
        /// 패턴 예측 (단순 확률 기반)
        /// </summary>
        public RpsMove PredictNextMove()
        {
            // 가장 자주 내는 손을 예측
            return GetMostFrequentMove();
        }

        /// <summary>
        /// 마크로 체인 확인 (연속 패턴)
        /// </summary>
        public bool HasConsecutivePattern(RpsMove move, int count)
        {
            if (_moveHistory.Count < count) return false;

            int consecutiveCount = 0;
            for (int i = _moveHistory.Count - 1; i >= 0; i--)
            {
                if (_moveHistory[i] == move)
                {
                    consecutiveCount++;
                    if (consecutiveCount >= count) return true;
                }
                else
                {
                    break;
                }
            }

            return false;
        }

        /// <summary>
        /// 기록 리셋
        /// </summary>
        public void ResetHistory()
        {
            _moveHistory.Clear();
            _rockCount = 0;
            _paperCount = 0;
            _scissorsCount = 0;
            Debug.Log("[PatternAnalyzer] History reset");
        }

        /// <summary>
        /// 통계 정보 문자열 반환
        /// </summary>
        public string GetStatisticsString()
        {
            if (TotalMoves == 0)
                return "No data yet";

            return $"Pattern: Rock {RockPercentage:F1}% | Paper {PaperPercentage:F1}% | Scissors {ScissorsPercentage:F1}%";
        }
    }
}
