using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

# if UNITY_EDITOR
[CustomEditor(typeof(GrassEditor))]
public class GrassPlacerEditor : Editor
{
    public float randomSizeRangeMin = 0.8f;
    public float randomSizeRangeMax = 1.2f;

    string minS = "0.8";
    string maxS = "1.2";
    string grassParent = "GrassIsland3"; // Grass will be parented to

    public override void OnInspectorGUI()
    {
        GrassEditor grass = target.GetComponent<GrassEditor>();
        if (grass == null) { return; }
        Transform placePoint = grass.transform.Find("PlacePoint");
        if (placePoint == null) { return; }

        minS = GUILayout.TextField(minS, 8);
        maxS = GUILayout.TextField(maxS, 8);

        if (GUILayout.Button("Place"))
        {
          
            Debug.Log(float.TryParse(minS,NumberStyles.Float,CultureInfo.InvariantCulture, out randomSizeRangeMin));
            float.TryParse(maxS, NumberStyles.Float,CultureInfo.InvariantCulture, out randomSizeRangeMax);

            Ray _ray = new Ray(placePoint.position, Vector3.down);
            Physics.Raycast(_ray, out RaycastHit _hit);
            grass.transform.rotation = Quaternion.LookRotation(-_hit.normal);
            float _zRotRandom = Random.Range(0, 361);
            Vector3 _eul = grass.transform.rotation.eulerAngles;
            _eul.z = _zRotRandom;
            grass.transform.rotation = Quaternion.Euler(_eul);
            GameObject _grassHolder = GameObject.Find(grassParent);
            grass.transform.position = new Vector3(_hit.point.x,_hit.point.y-0.001f, _hit.point.z);
            grass.transform.localScale = new Vector3(Random.Range(randomSizeRangeMin, randomSizeRangeMax),
                Random.Range(randomSizeRangeMin, randomSizeRangeMax),
                Random.Range(randomSizeRangeMin, randomSizeRangeMax));
            Debug.Log(randomSizeRangeMin);
            if(_grassHolder != null)
            {
                grass.transform.SetParent(_grassHolder.transform);
            }
            else
            {
                Debug.LogError("No main Grass gameObject!");
            }
          
        }

    }
     

}
#endif
