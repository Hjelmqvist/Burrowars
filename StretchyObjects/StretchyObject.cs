using UnityEngine;

/// <summary>
/// This Class defines objects capable of stretching between two transforms in 3d space.
/// Allways place these under a parent object for stabillity reasons.
/// 
/// Heavily inspired by Thinkyhead
/// http://www.thinkyhead.com/blogs/thinkyhead/2015-11-18/stretchy-objects-unity-3d
/// </summary>
public class StretchyObject : MonoBehaviour
{
    [SerializeField] protected Vector3[] targetPoints, oldTargetPoints;
    [SerializeField] protected Vector3[] targetOffsets = new Vector3[2];
    [SerializeField] protected Transform[] targetObjs = new Transform[2];
    [SerializeField] protected float scalefactor = 1;

    protected Vector3[] endPoints
    {
        get
        {
            Vector3 pos = transform.position + Vector3.forward,
                    ray = transform.forward * transform.lossyScale.z / 2;
            return new Vector3[2] { pos - ray, pos + ray };
        }
    }

    //Set up the object in awake if two targetsObjs have been set in the inspector
    protected virtual void Awake()
    {
        TetherToEndPoint(1, targetObjs[1], targetOffsets[1]);
        TetherToEndPoint(0, targetObjs[0], targetOffsets[0]);
        targetPoints = endPoints;
        oldTargetPoints = endPoints;
    }

    /// <summary>
    /// This method links one of the stretchyobjects enpoints to a given transform in 3d space.
    /// </summary>
    /// <param name="index">The index corresponding to the endpoints. Can only be 0 or 1.</param>
    /// <param name="target">The target transform to link to.</param>
    /// <param name="offset">The offset from the target transform origo to stretch towards.</param>
    /// <returns>True if the index is allowed.</returns>
    protected bool TetherToEndPoint(int index, Transform target, Vector3 offset)
    {
        if(index == 0 || index == 1)
        {
            targetObjs[index] = target;
            targetOffsets[index] = offset;
            return true;
        }
        return false;
    }

    private void Update()
    {
        for (int i = 0; i < 2; i++)
            if (targetObjs != null)
                targetPoints[i] = targetObjs[i].position + targetOffsets[i];

        bool didMove = false;
        Vector3[] targetLocalPos = new Vector3[2];

        for (int i = 0; i < 2; i++)
        {
            Vector3 tlp = targetPoints[i];
            if (transform.parent != null)   //Use the parent transform to stop weird stuff from happening.
                tlp = transform.parent.InverseTransformPoint(tlp);
            targetLocalPos[i] = tlp;
            if (oldTargetPoints[i] != tlp)
            {
                oldTargetPoints[i] = tlp;
                didMove = true;
            }
        }
        if (!didMove) return;   //Save some calculations if the transforms we are tethered to did not move, as there will be no need to scale the object.

        Vector3 targetDiff = targetLocalPos[1] - targetLocalPos[0];
        Vector3 localScale = transform.localScale;
        localScale.z = targetDiff.magnitude;
        transform.localScale = localScale;
        transform.localPosition = (targetLocalPos[0] + targetLocalPos[1]) / 2f;
        transform.localRotation = Quaternion.LookRotation(targetDiff);
    }
}
