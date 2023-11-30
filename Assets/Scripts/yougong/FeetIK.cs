using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetIK : MonoBehaviour
{
    // Start is called before the first frame update

    private Animator animator;
    [SerializeField] float distanceGround = 0.2f;
    public LayerMask layerMask;
    void Awake()
    {
	    animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    		if (animator)
    		{
    			// Left Foot
    			// Position 과 Rotation weight 설정
    			animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
    			animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
    
    			///<summary>
    			/// GetIKPosition 
    			///   => IK를 하려는 객체의 위치 값 ( 아래에선 아바타에서 LeftFoot에 해당하는 객체의 위치 값 )
    			/// Vector3.up을 더한 이유 
    			///   => origin의 위치를 위로 올려 바닥에 겹쳐 바닥을 인식 못하는 걸 방지하기 위해
    			///      (LeftFoot이 발목 정도에 있기 때문에 발바닥과 어느 정도 거리가 있고, Vector3.up을 더해주지 않으면 발목 기준으로 처리가 되어 발 일부가 바닥에 들어간다.)
    			///</summary>
    			Ray leftRay = new Ray(animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
    
    			// distanceGround: LeftFoot에서 땅까지의 거리
    			// +1을 해준 이유: Vector3.up을 해주었기 때문
    			if (Physics.Raycast(leftRay, out RaycastHit leftHit, distanceGround + 1f, layerMask))
    			{
    				// 걸을 수 있는 땅이라면
    				if (leftHit.transform.tag == "WalkableGround")
    				{
    					Vector3 footPosition = leftHit.point;
    					footPosition.y += distanceGround;
    
    					animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
    					animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, leftHit.normal));
    				}
    			}
    
    			// Right Foot
    			animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
    			animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
    
    			Ray rightRay = new Ray(animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
    
    			if (Physics.Raycast(rightRay, out RaycastHit rightHit, distanceGround + 1f, layerMask))
    			{
    				if (rightHit.transform.tag == "WalkableGround")
    				{
    					Vector3 footPosition = rightHit.point;
    					footPosition.y += distanceGround;
    
    					animator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
    					animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rightHit.normal));
    				}
    			}
    		}
    }
}
