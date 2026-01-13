using UnityEngine;
using System;

namespace RPS.Skills
{
    /// <summary>
    /// 매치 관리 Skill - HP 배틀 시스템
    /// </summary>
    public class MatchManager : MonoBehaviour
    {
        [Header("HP Settings")]
        [SerializeField] private int _maxHP = 30;
        [SerializeField] private int _damagePerRound = 10;

        [Header("Current Match State")]
        [SerializeField] private int _currentRound = 0;
        [SerializeField] private int _playerHP = 30;
        [SerializeField] private int _opponentHP = 30;

        // 이벤트
        public event Action<int> OnRoundStarted;
        public event Action<int, int> OnRoundEnded; // roundNumber, result (1=win, 0=draw, -1=lose)
        public event Action<int, int> OnHPChanged; // playerHP, opponentHP
        public event Action<MatchResult> OnMatchEnded;

        /// <summary>
        /// 현재 라운드 번호 (1부터 시작)
        /// </summary>
        public int CurrentRound => _currentRound;

        /// <summary>
        /// 플레이어 현재 HP
        /// </summary>
        public int PlayerHP => _playerHP;

        /// <summary>
        /// 상대 현재 HP
        /// </summary>
        public int OpponentHP => _opponentHP;

        /// <summary>
        /// 최대 HP
        /// </summary>
        public int MaxHP => _maxHP;

        /// <summary>
        /// 매치가 종료되었는지 확인
        /// </summary>
        public bool IsMatchEnded => _playerHP <= 0 || _opponentHP <= 0;

        private void Awake()
        {
            Debug.Log($"[MatchManager] Initialized - HP Battle System (Max HP: {_maxHP}, Damage: {_damagePerRound})");
        }

        private void Start()
        {
            StartMatch();
        }

        /// <summary>
        /// 새 매치 시작
        /// </summary>
        public void StartMatch()
        {
            _currentRound = 0;
            _playerHP = _maxHP;
            _opponentHP = _maxHP;

            Debug.Log($"[MatchManager] New match started - Player HP: {_playerHP}, Opponent HP: {_opponentHP}");
            OnHPChanged?.Invoke(_playerHP, _opponentHP);
            StartNextRound();
        }

        /// <summary>
        /// 다음 라운드 시작
        /// </summary>
        public void StartNextRound()
        {
            if (IsMatchEnded)
            {
                Debug.LogWarning("[MatchManager] Match already ended");
                return;
            }

            _currentRound++;
            Debug.Log($"[MatchManager] Round {_currentRound} started (Player: {_playerHP} HP, Opponent: {_opponentHP} HP)");
            OnRoundStarted?.Invoke(_currentRound);
        }

        /// <summary>
        /// 라운드 결과 처리
        /// </summary>
        /// <param name="result">1=플레이어 승리, 0=무승부, -1=플레이어 패배</param>
        public void EndRound(int result)
        {
            if (result > 0)
            {
                // 플레이어 승리: 상대 HP 차감
                _opponentHP = Mathf.Max(0, _opponentHP - _damagePerRound);
                Debug.Log($"[MatchManager] Player wins round {_currentRound}! Opponent takes {_damagePerRound} damage! (Player: {_playerHP} HP, Opponent: {_opponentHP} HP)");
            }
            else if (result < 0)
            {
                // 상대 승리: 플레이어 HP 차감
                _playerHP = Mathf.Max(0, _playerHP - _damagePerRound);
                Debug.Log($"[MatchManager] Opponent wins round {_currentRound}! Player takes {_damagePerRound} damage! (Player: {_playerHP} HP, Opponent: {_opponentHP} HP)");
            }
            else
            {
                // 무승부: HP 변화 없음
                Debug.Log($"[MatchManager] Round {_currentRound} is a draw! No damage dealt. (Player: {_playerHP} HP, Opponent: {_opponentHP} HP)");
            }

            OnRoundEnded?.Invoke(_currentRound, result);
            OnHPChanged?.Invoke(_playerHP, _opponentHP);

            // 매치 종료 확인
            CheckMatchEnd();
        }

        /// <summary>
        /// 매치 종료 확인
        /// </summary>
        private void CheckMatchEnd()
        {
            if (!IsMatchEnded) return;

            MatchResult matchResult;

            if (_playerHP <= 0 && _opponentHP <= 0)
            {
                // 동시에 HP가 0이 된 경우 (드물지만 가능)
                matchResult = MatchResult.Defeat; // 패배로 처리
                Debug.Log("[MatchManager] Double KO! Both players defeated simultaneously.");
            }
            else if (_playerHP <= 0)
            {
                matchResult = MatchResult.Defeat;
                Debug.Log($"[MatchManager] Player defeated! Final HP - Player: {_playerHP}, Opponent: {_opponentHP}");
            }
            else
            {
                matchResult = MatchResult.Victory;
                Debug.Log($"[MatchManager] Player victorious! Final HP - Player: {_playerHP}, Opponent: {_opponentHP}");
            }

            OnMatchEnded?.Invoke(matchResult);
        }

        /// <summary>
        /// HP 직접 설정 (디버깅용)
        /// </summary>
        public void SetHP(int playerHP, int opponentHP)
        {
            _playerHP = Mathf.Clamp(playerHP, 0, _maxHP);
            _opponentHP = Mathf.Clamp(opponentHP, 0, _maxHP);

            Debug.Log($"[MatchManager] HP set manually - Player: {_playerHP}, Opponent: {_opponentHP}");
            OnHPChanged?.Invoke(_playerHP, _opponentHP);
        }

        /// <summary>
        /// 매치 리셋
        /// </summary>
        public void ResetMatch()
        {
            _currentRound = 0;
            _playerHP = _maxHP;
            _opponentHP = _maxHP;

            Debug.Log("[MatchManager] Match reset");
            OnHPChanged?.Invoke(_playerHP, _opponentHP);
        }
    }
}
