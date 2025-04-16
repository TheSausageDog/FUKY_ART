using UnityEngine;

//Just moving
public class NormalPickableItem : PickableItem
{
    public Rigidbody itemRigidbody { get; protected set; }

    protected Transform holdPos;

    public override void Awake()
    {
        base.Awake();
        itemRigidbody = GetComponent<Rigidbody>();
    }

    public virtual void Update()
    {
        if (isPicking)
        {
            // 计算目标位置与当前物体位置之间的距离
            float distanceToTarget = Vector3.Distance(transform.position, holdPos.position);

            // 计算目标位置与当前物体位置之间的方向
            Vector3 directionToTarget = (holdPos.position - transform.position).normalized;

            // 动态调整速度：距离越远，速度越快
            float dynamicSpeed = PlayerBlackBoard.handSpeed * distanceToTarget;

            // 计算目标速度
            Vector3 targetVelocity = directionToTarget * dynamicSpeed;

            // 设置刚体速度
            itemRigidbody.velocity = targetVelocity;

            // // 同时调整手的位置，使其始终与物体位置对齐
            // transform.position = transform.position - pickUpPos.position + transform.position;

            // // 保持手的旋转
            // transform.rotation = handTarget.rotation;
        }
    }

    public override void OnPickup(Transform _holdPos)
    {
        holdPos = _holdPos;
        SetPickedRigidbody(itemRigidbody);
        base.OnPickup(_holdPos);
    }

    public override void OnThrow()
    {
        holdPos = null;
        SetDropRigidbody(itemRigidbody);
        base.OnThrow();
    }
}