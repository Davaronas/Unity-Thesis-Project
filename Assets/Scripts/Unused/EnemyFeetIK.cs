using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class EnemyFeetIK : MonoBehaviour
{
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform leftFootTarget;
    [SerializeField] private Rig leftLegRig;
    [SerializeField] private MultiParentConstraint leftFootConstraint;
    [Space]
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform rightFootTarget;
    [SerializeField] private Rig rightLegRig;
    [SerializeField] private MultiParentConstraint rightFootConstraint;
    [Space]
    [SerializeField] private Transform hip;
    [SerializeField] private Vector3 footAdditionalRot;

    [SerializeField] private float resetDistance = 1f;


    

    private Animator animator;
    private EnemyBehaviour eb;

    private bool leftLegHasTarget = false;
    private bool rightLegHasTarget = false;

    

    private Vector3 leftFootOriginalPos;
    private Vector3 rightFootOriginalPos;

    private LayerMask layersHit;

    RaycastHit hit;
    Ray ray;

    [SerializeField] private float raycastOriginUp = 0.1f;
   [SerializeField][Range(0, 5)] private float distanceToGround = 0.05f;


    private void Start()
    {
        layersHit = LayerMask.GetMask("Default");
        animator = GetComponent<Animator>();
        eb = GetComponent<EnemyBehaviour>();

        leftFootOriginalPos = leftFoot.position;
        rightFootOriginalPos = rightFoot.position;

       // StartCoroutine(LeftFootIK());
       // StartCoroutine(RightFootIK());
    }

    private void Update()
    {
        if(!leftLegHasTarget)
        {
            leftLegRig.weight = 0f;
            leftFootOriginalPos = leftFoot.position;
            leftFootConstraint.transform.position = leftFootOriginalPos;
            leftFootConstraint.weight = 1f;
        }

        if(!rightLegHasTarget)
        {
            rightLegRig.weight = 0f;
            rightFootOriginalPos = rightFoot.position;
            rightFootConstraint.transform.position = rightFootOriginalPos;
            rightFootConstraint.weight = 1f;
        }
       
      
    }

    private void LateUpdate()
    {
        if(eb.IsIncapacitated()) { leftLegHasTarget = false; rightLegHasTarget = false; return; }

        /*
        leftLegRig.weight = 0;
        leftLegConstraint.weight = 1;

        leftFootOriginalPos = leftLegConstraint.transform.position;
        leftFootTarget.position = leftFootOriginalPos;
        */

        ray = new Ray(leftFootOriginalPos + (Vector3.up * raycastOriginUp), Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, out hit, distanceToGround, layersHit))
        {
            leftFootTarget.position = hit.point;
            leftFootTarget.rotation = Quaternion.LookRotation(hit.normal, hip.forward + footAdditionalRot);
            leftLegRig.weight = 1;
            leftLegHasTarget = true;
            leftFootConstraint.weight = 0;
        }
        else
        {
            leftLegHasTarget = false;
        }



        /*
        rightLegRig.weight = 0;
        rightLegConstraint.weight = 1;

        rightFootOriginalPos = rightLegConstraint.transform.position;
        rightFootTarget.position = rightFootOriginalPos;
        */

        ray = new Ray(rightFootOriginalPos + (Vector3.up * raycastOriginUp), Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, out hit, distanceToGround, layersHit))
        {
            rightFootTarget.position = hit.point;
            rightFootTarget.rotation = Quaternion.LookRotation(hit.normal, hip.forward + footAdditionalRot);
            rightLegRig.weight = 1;
            rightLegHasTarget = true;
            rightFootConstraint.weight = 0;
        }
        else
        {
            rightLegHasTarget = false;
        }



        if (Vector3.Magnitude(leftFoot.position - leftFootConstraint.transform.position) > resetDistance)
        {
            leftLegRig.weight = 0f;
            leftFootOriginalPos = leftFoot.position;
            leftLegHasTarget = false;
            leftFootConstraint.transform.position = leftFootOriginalPos;
            leftFootConstraint.weight = 1f;
        }

        if (Vector3.Magnitude(rightFoot.position - rightFootConstraint.transform.position) > resetDistance)
        {
            rightLegRig.weight = 0f;
            rightFootOriginalPos = rightFoot.position;
            rightLegHasTarget = false;
            rightFootConstraint.transform.position = rightFootOriginalPos;
            rightFootConstraint.weight = 1f;
        }

    }

    IEnumerator LeftFootIK()
    {
        while (!eb.IsIncapacitated())
        {
            /*
            if (Vector3.Magnitude(transform.position - leftFoot.position) > resetDistance)
            {
                print("wtf reset");
                leftLegRig.weight = 0f;
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                leftFootOriginalPos = leftFoot.position;
            }
            else
            {
                ray = new Ray(leftFootOriginalPos + (Vector3.up * raycastOriginUp), Vector3.down);
                Debug.DrawRay(ray.origin, ray.direction);
                if (Physics.Raycast(ray, out hit, distanceToGround, layersHit))
                {
                    print("wtf2");
                    leftFootTarget.position = hit.point;
                }
                leftLegRig.weight = 1f;
            }
            */

            leftLegRig.weight = 0f;
            yield return new WaitForEndOfFrame();
            leftFootOriginalPos = leftFoot.position;

            ray = new Ray(leftFootOriginalPos + (Vector3.up * raycastOriginUp), Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hit, distanceToGround, layersHit))
            {
                print("wtf2");
                leftFootTarget.position = hit.point;
            }
            leftLegRig.weight = 1f;


            yield return null;
        }

        yield return null;
    }

    IEnumerator RightFootIK()
    {
        while (!eb.IsIncapacitated())
        {
            if (Vector3.Magnitude(transform.position - rightFoot.position) > resetDistance)
            {
                print("wtf reset");
                rightLegRig.weight = 0f;
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                rightFootOriginalPos = rightFoot.position;
            }


            ray = new Ray(rightFootOriginalPos + (Vector3.up * raycastOriginUp), Vector3.down);
            Debug.DrawRay(ray.origin, ray.direction);
            if (Physics.Raycast(ray, out hit, distanceToGround, layersHit))
            {
                print("wtf2");
                rightFootTarget.position = hit.point;
            }
            rightLegRig.weight = 1f;

            yield return null;
        }

        yield return null;
    }


}
