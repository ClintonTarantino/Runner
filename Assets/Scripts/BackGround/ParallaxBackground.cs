using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour {
    public GameObject referenceObject;
    public float elementSize;
    public float elementOffset;

    private List<GameObject> backGroundElements;

	// Use this for initialization
	void Start () {
        backGroundElements = new List<GameObject>();
        for(int i = 0; i<transform.childCount; i++) {
           backGroundElements.Add(transform.GetChild(i).gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		foreach(GameObject backgroundElement in backGroundElements) {
          if(referenceObject.transform.position.x - backgroundElement.transform.position.x > elementOffset) {
                backgroundElement.transform.position = new Vector3(
                    backgroundElement.transform.position.x + backGroundElements.Count * elementSize,
                    backgroundElement.transform.position.y,
                    backgroundElement.transform.position.z      
                );
            }      
        }
	}
}
