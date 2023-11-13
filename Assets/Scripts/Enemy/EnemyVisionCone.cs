using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisionCone : MonoBehaviour
{
    private PlayerActions player;
    private EnemyCommands thisEnemy;

    private List<EnemyBody> enemyBodies;
    private List<EnemyCommands> otherEnemiesInSight;

    private PlayerSpotPosition[] playerSpotPositions;

    private Ray visionRay;
    private int visionPointsVisibleThisFrame = 0;

    private Ray enemyBodiesRay;
    private float enemyBodiesPointCalc;

    private bool scanForPlayer = false;

   [SerializeField] private LayerMask playerScanLayerMasks;
   [SerializeField] private LayerMask enemyBodyScanLayerMasks;

    private Transform endPoint;
    private float maxSpotDistance;

    private float spotPoints = 0;

    private bool stopAllSignal = false;

    private class EnemyBody
    {
        public Transform body;
        public EnemyIncSpotPoint[] spotPoints;
        public float pointsAccumulated;

        public EnemyBody(Transform _b)
        {
            body = _b;
            spotPoints = _b.GetComponentsInChildren<EnemyIncSpotPoint>();
            if (spotPoints == null)
            {
                Debug.LogError($"New EnemyBody Class {_b} doesn't have any EnemyIncSpotPoints in its children");
                if (spotPoints.Length < 1)
                {
                    Debug.LogError($"New EnemyBody Class {_b} doesn't have any EnemyIncSpotPoints in its children");
                }
            }
            pointsAccumulated = 0;
        }

        public override bool Equals(object obj)
        {
            return obj is EnemyBody body &&
                   EqualityComparer<Transform>.Default.Equals(this.body, body.body);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(body);
        }

        public static bool operator ==(EnemyBody _eb1, EnemyBody _eb2)
        {
            if(_eb1.body == _eb2.body)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(EnemyBody _eb1, EnemyBody _eb2) => !(_eb1 == _eb2);


        public static implicit operator EnemyBody(Transform _t) => new EnemyBody(_t);
        
    }

    
    public List<EnemyCommands> GetOtherEnemiesInSight()
    {
        return otherEnemiesInSight;
    }
    

    private void Start()
    {
        player = FindObjectOfType<PlayerActions>();
        thisEnemy = transform.root.GetComponent<EnemyCommands>();

        playerSpotPositions = FindObjectsOfType<PlayerSpotPosition>();
        

      // playerScanLayerMasks = LayerMask.GetMask("PlayerVisualSpot","Default", "ShadowWall");
      //  enemyBodyScanLayerMasks = LayerMask.GetMask("EnemyIncSpotPoints", "Default", "ShadowWall");

        endPoint = transform.GetChild(0);
        maxSpotDistance = (transform.position - endPoint.position).magnitude;

        enemyBodies = new List<EnemyBody>();

        PlayerActions.OnPlayerDash += ForceStopScan;
        PlayerActions.OnPlayerDied += PlayerDiedReceiver;

        otherEnemiesInSight = new List<EnemyCommands>();
    }



    private void OnDestroy()
    {
        PlayerActions.OnPlayerDash -= ForceStopScan;
        PlayerActions.OnPlayerDied -= PlayerDiedReceiver;
    }


    private void ForceStopScan()
    {
        scanForPlayer = false;
    }

    private void PlayerDiedReceiver()
    {
        stopAllSignal = true;
    }

    private void FixedUpdate()
    {
        if(stopAllSignal || thisEnemy.IsIncapacitated()) { return; }

        spotPoints = 0;

        if (scanForPlayer) //player
        {
            EvaluatePlayerVisualPoints();
        }

        if(enemyBodies.Count > 0)
        {
            if ( thisEnemy.GetAlertnessPercent() >= 1f) { return; }

            for(int i = 0; i < enemyBodies.Count;i++)
            {
                //  print(enemyBodies[i].pointsAccumulated + " accumulated");
                if(enemyBodies[i].pointsAccumulated >= thisEnemy.GetMaxVisualPointsPerCorpse()) { continue; }

                for (int j = 0; j < enemyBodies[i].spotPoints.Length;j++)
                {


                    enemyBodiesRay = new Ray(transform.position, enemyBodies[i].spotPoints[j].transform.position - transform.position);
                    if(Physics.Raycast(enemyBodiesRay, out RaycastHit _hit, maxSpotDistance,enemyBodyScanLayerMasks))
                    {


                        if (_hit.collider.gameObject.layer == 15) // EnemyIncSpot, only this layer should gather points
                        {
                            enemyBodiesPointCalc = thisEnemy.GetCorpseVisualMultiplier() *
                                (1 - ((enemyBodies[i].spotPoints[j].transform.position - transform.position).magnitude / maxSpotDistance));


                            enemyBodies[i].pointsAccumulated += enemyBodiesPointCalc;

                            thisEnemy.ReceiveAwarenessSignal(enemyBodiesPointCalc, enemyBodies[i].body.position, true, false);
                            break;
                        }
                    }
                }
            }
        }


    }

    private void EvaluatePlayerVisualPoints()
    {
        visionPointsVisibleThisFrame = 0;

        for (int i = 0; i < playerSpotPositions.Length; i++)
        {
            visionRay = new Ray(transform.position, playerSpotPositions[i].transform.position - transform.position);
            if (Physics.Raycast(visionRay, out RaycastHit hit, Mathf.Infinity, playerScanLayerMasks))
            {
                if (hit.collider.gameObject.layer == 9)
                {
                    visionPointsVisibleThisFrame++;
                }
            }
        }

        if (visionPointsVisibleThisFrame <= 0) { return; }

        spotPoints = visionPointsVisibleThisFrame * (1 - ((player.transform.position - transform.position).magnitude / maxSpotDistance));

        if (player.IsSneaking())
        {
            spotPoints *= player.SneakingVisualReduction();
        }

        if (player.IsUnderSomething())
        {
            spotPoints *= player.PlayerUnderAnObjectVisualReduction();
        }

        if (player.IsHeightDifferenceSignificant(thisEnemy.transform.position))
        {
            spotPoints *= player.HeightDifferenceVisualReduction();
        }

        if (player.IsShadowFormActive())
        {
            spotPoints *= player.GetShadowFormVisualReduction();
        }

        //  print(spotPoints);
        if (spotPoints > 0.2f)
        {
            thisEnemy.ReceiveAwarenessSignal(spotPoints);
        }
    }

    public bool IsPlayerInVision()
    {
        if(spotPoints > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8) //player
        {
            scanForPlayer = true;
        }

        if(other.gameObject.layer == 14) // Dead enemy (EnemyInc)
        {
            if (!enemyBodies.Contains(other.transform))
            {
                enemyBodies.Add(other.transform);
            }
        }

        if(other.gameObject.layer == 6) // Alive enemy
        {
           EnemyBehaviour _enemyInSight = other.GetComponent<EnemyBehaviour>();

           if (_enemyInSight == thisEnemy) { return; }

           if (otherEnemiesInSight.Contains(_enemyInSight)) { return; }

            otherEnemiesInSight.Add(_enemyInSight);

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8) //player
        {
            scanForPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8) //player
        {
            scanForPlayer = false;
        }


        if (other.gameObject.layer == 14) // Dead enemy (EnemyInc)
        {
            enemyBodies.Remove(other.transform);
        }

        if (other.gameObject.layer == 6) // Alive enemy
        {
            EnemyCommands _ec = other.GetComponent<EnemyCommands>();
            otherEnemiesInSight.Remove(_ec);
        }
    }
}
