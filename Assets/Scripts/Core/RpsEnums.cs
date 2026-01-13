using UnityEngine;

namespace RPS
{
    /// <summary>
    /// 가위바위보 손 모양 enum
    /// </summary>
    public enum RpsMove
    {
        None,
        Rock,      // 바위
        Paper,     // 보
        Scissors   // 가위
    }

    /// <summary>
    /// 라운드 결과 enum
    /// </summary>
    public enum RoundResult
    {
        None,
        Win,       // 승리
        Lose,      // 패배
        Draw       // 무승부
    }

    /// <summary>
    /// 매치 결과 enum
    /// </summary>
    public enum MatchResult
    {
        None,
        Victory,   // 매치 승리 (2선승)
        Defeat     // 매치 패배
    }

    /// <summary>
    /// 게임 상태 enum
    /// </summary>
    public enum GameState
    {
        None,
        Idle,          // 대기 상태
        Countdown,     // 카운트다운
        Choosing,      // 선택 중
        Revealing,     // 결과 공개
        RoundEnd,      // 라운드 종료
        MatchEnd       // 매치 종료
    }
}
