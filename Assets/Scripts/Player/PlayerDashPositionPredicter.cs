using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashPositionPredicter : MonoBehaviour
{

    private Transform p;
    private CharacterController cc;
    private LineRenderer pLR;
    private MeshRenderer mr;

    void Start()
    {
        p = FindObjectOfType<PlayerActions>().transform.Find("DashAimPos");
        cc = GetComponent<CharacterController>();
        pLR = p.GetComponent<LineRenderer>();
        mr = transform.GetComponentInChildren<MeshRenderer>();
    }

    void Update()
    {
       

        if (p.gameObject.activeSelf)
        {
            if (!mr.enabled)
            {
                mr.enabled = true;
            }

            cc.enabled = false;

            transform.Translate((p.position - transform.position) - (Vector3.down / 2));

            cc.enabled = true;


               cc.Move(Vector3.up/10);

            pLR.SetPosition(1, p.InverseTransformPoint(transform.position + (Vector3.down / 2)));
            mr.transform.Rotate(Vector3.up*1.5f);
        }
        else
        {
            if(mr.enabled)
            {
                mr.enabled = false;
            }
        }
    }

  

}
