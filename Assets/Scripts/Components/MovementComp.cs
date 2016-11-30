using GameGHJ.Common.ZenECS;

public class MovementComp : ComponentECS
{
    public float moveSpeed = 5;
    public float currentMoveSpeed = 0;
    public MovementType movementType = MovementType.Ground;

    public MovementComp() : base()
    {
    }

  

    public override ComponentTypes ComponentType => ComponentTypes.MovementComp;
}

public enum MovementType
{
    Ground,
    Air
}