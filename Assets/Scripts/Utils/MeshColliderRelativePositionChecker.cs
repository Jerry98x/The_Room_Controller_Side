using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MeshColliderRelativePositionChecker : MonoBehaviour
{
 
    
    [SerializeField] private MeshCollider meshCollider;
    
    private bool inMeshCollider;
    private bool concaveHull;
    private float distance = Mathf.Infinity;

	Ray right, left, up, down, forward, back, tempRay;
	bool r, l, u, d, f, b;

	RaycastHit rightHit   = new RaycastHit();
	RaycastHit leftHit    = new RaycastHit();
	RaycastHit upHit      = new RaycastHit();
	RaycastHit downHit    = new RaycastHit();
	RaycastHit forwardHit = new RaycastHit();
	RaycastHit backHit    = new RaycastHit();
	RaycastHit tempHit    = new RaycastHit();

	void Start(){

		right   = new Ray(Vector3.zero , -Vector3.right);
		left    = new Ray(Vector3.zero , -Vector3.left);
		up      = new Ray(Vector3.zero , -Vector3.up);
		down    = new Ray(Vector3.zero , -Vector3.down);
		forward = new Ray(Vector3.zero , -Vector3.forward);
		back    = new Ray(Vector3.zero , -Vector3.back);
		tempRay = new Ray();

	}

	bool ConcaveHull(Ray ray, RaycastHit hit){


		tempRay.origin = transform.position;
		tempRay.direction = -ray.direction;
		float customDistance = distance-hit.distance;
		int lastPoint = hit.triangleIndex;

		while(meshCollider.Raycast(tempRay, out tempHit, customDistance)){

			if(tempHit.triangleIndex == lastPoint) break;
			lastPoint = tempHit.triangleIndex;
			customDistance = tempHit.distance;
			ray.origin = -ray.direction * customDistance + transform.position;

			if(!meshCollider.Raycast(ray, out tempHit, customDistance)) {

				concaveHull = true;
				return true;

			}

			if(tempHit.triangleIndex == lastPoint) break;
			lastPoint = tempHit.triangleIndex;
			customDistance -= tempHit.distance;

		}

		return false;

	}

	// Update is called once per frame
	void Update () {
	
		right.origin   = -right.direction   * distance + transform.position;
		left.origin    = -left.direction    * distance + transform.position;
		up.origin      = -up.direction      * distance + transform.position;
		down.origin    = -down.direction    * distance + transform.position;
		forward.origin = -forward.direction * distance + transform.position;
		back.origin    = -back.direction    * distance + transform.position;

		r = meshCollider.Raycast(right   , out rightHit   , distance);
		l = meshCollider.Raycast(left    , out leftHit    , distance);
		u = meshCollider.Raycast(up      , out upHit      , distance);
		d = meshCollider.Raycast(down    , out downHit    , distance);
		f = meshCollider.Raycast(forward , out forwardHit , distance);
		b = meshCollider.Raycast(back    , out backHit    , distance);

		if(r && l && u && d && f && b) {

			if(ConcaveHull(right,rightHit))          inMeshCollider = false;
			else if(ConcaveHull(left,leftHit))       inMeshCollider = false;
			else if(ConcaveHull(up,upHit))           inMeshCollider = false;
			else if(ConcaveHull(down,downHit))       inMeshCollider = false;
			else if(ConcaveHull(forward,forwardHit)) inMeshCollider = false;
			else if(ConcaveHull(back,backHit))       inMeshCollider = false;
			else { inMeshCollider = true; concaveHull = false; }

		} else inMeshCollider=false;
	
	}
    
    public bool IsInMeshCollider(){
		return inMeshCollider;
	}
}
