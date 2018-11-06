using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCursor : MonoBehaviour
{
	public static GameCursor instance;
    public float gizmoSize;

	void Awake(){
		if(instance==null){
			instance=this;
		}else if(instance!=this){
			Destroy(gameObject);
			return;
		}
	}

    void Update()
    {
        if (Input.GetMouseButton(1)) //RMB
        {
            Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosWorld.x, mousePosWorld.y, 0);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}
