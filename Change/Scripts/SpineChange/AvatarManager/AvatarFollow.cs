using UnityEngine;
using Spine.Unity;
using Spine;

public class AvatarFollow : MonoBehaviour {
	private bool isStart;
	private Transform roleTrans;
	private BoneFollower roleBoneFollower;
	private Transform followTrans;
	private SpineAvatar followAvatar;
	public void StartFollow(GameObject role,SpineAvatar follow){
		followAvatar = follow;
		roleBoneFollower = role.GetComponent<BoneFollower>();
		followTrans = ((followAvatar.avatarObject) as GameObject).transform;
		roleTrans = role.transform;
		isStart = true;
	}
	private void Update () {
		if(isStart){
			followTrans.position = roleTrans.position;
			// + new Vector3(-4.5f, -3.5f, 0);
			//roleBoneFollower.bone.GetWorldPosition(roleTrans);
		}
	}
	private void OnDestroy(){
		Destroy(roleBoneFollower);
		Destroy(roleTrans.gameObject);
		followAvatar.DestroySpineAvatar();
	}
}