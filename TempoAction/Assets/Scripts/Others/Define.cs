

public class Define
{
    public enum BuffType // ENTER : ������ ���� ���� , STAY : ������ �� ������ ����, EXIT : �������� ������ ���� 
    {
        ENTER, STAY, EXIT
    }

    public enum BuffInfo
    {
        SUPERJUMP,

        #region buff
        SPEEDUP,
        POWERUP,
        HEAL,
        #endregion

        #region debuff
        TICKDAMAGE,

        #endregion
        NONE
    }

    public enum UIType
    {
        ALL, BUTTON, IMAGE, CANVASGROUP, TMPRO, TOGGLE, SLIDER
    }
    public enum TempoType
    {
        MAIN, POINT, NONE
    }

    public enum AtkState
    {
        ATTACK, CHECK, FINISH
    }

    public enum PlayerState
    {
        OVERLOAD, STUN, NONE
    }

    public enum CircleState
    {
        MISS, BAD, GOOD, PERFECT, NONE
    }

    public enum PlayerSfxType
    {
        MAIN, POINT, DASH, RUN, JUMP, STUN, NONE
    }
}
