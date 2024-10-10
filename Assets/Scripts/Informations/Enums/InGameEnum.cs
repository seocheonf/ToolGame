namespace ToolGame
{

    public enum ForceType
    {
        VelocityForce,
        ImpulseForce,
        DurationForce,
        UnityDuration
    }

    public enum CrowdControlState
    {
        ElectricShcok,
        Stun,
        Length
    }

    public enum GeneralState
    {
        Normal,
        CrowdControl,
        Action,
        Length
    }

    public enum CameraViewType
    {
        Custom,
        Default,
        FirstView,
        ThirdView,
        Length
    }

    public enum OuterKeyCode
    {
        
        #region About Playable
        /// <summary>
        /// 플레이어블과 관련된 입력. 항상 쓰고 있다고 생각해야 함.
        /// </summary>
        
        //앞
        Forward,
        //뒤
        Backward,
        //좌(ward붙인 이유는 단순 상하좌우와 개념적으로 비교하기 위함)
        Leftward,
        //우
        Rightward,

        //점프
        Jump,
        //대시
        Dash,
        //앉기
        Sit,
        //돌진
        Rush,
        //도구 취하기 (잡기, 놓기)
        TakeTool,
        //외부 상호작용
        OuterFunc,

        //Esc
        Esc,

        //인칭 전환
        Sight,

        #endregion

        #region About Tool

        /// <summary>
        /// 도구와 관련된 입력. 쓰고있는지, 아닌지 상황에 따라 파악해야 함.
        /// </summary>

        //뒤집기
        Reverse,
        //쏘기/날리기/휘두르기/펼치기 등과 같은 액션성이 강한 것들
        Action,
        //앞-좌 회전류
        Rot_Forward_Left,
        //뒤-우 회전류
        Rot_Backward_Right,

        #endregion

        T1,
        T2,
        T3,
        T4,
        T5,
        T6,
        T7,
        T8,
        T9,

        //길이 용도
        Length
    }


    public enum FixedUIType
    {
        FixedUITest,
        PlayableInputUI,
        PlayableInputUIBlock,

        TitleUI
    }

    public enum FloatingUIType
    {
        FloatingUITest,

        InStageOption
    }

}