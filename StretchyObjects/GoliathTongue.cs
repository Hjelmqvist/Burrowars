using UnityEngine;

/// <summary>
/// This class is used to stretch the goliathtongue towards the target player during the Goliaths grabbing attack.
/// </summary>
public class GoliathTongue : StretchyObject
{
    [SerializeField] Enemy owner = null;

    protected override void Awake()
    {
        TetherToEndPoint(1, owner.Target.transform, Vector3.up);
        TetherToEndPoint(0, owner.transform, Vector3.zero);
        targetPoints = endPoints;
        oldTargetPoints = endPoints;
    }
}
