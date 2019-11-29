using System;
using System.Collections;
using System.Collections.Generic;
using DancingWord.SpineChange;
using UnityEngine;

public class AvatarCreateQueue {
	private static AvatarCreateQueue _instance;
	public static AvatarCreateQueue Instance {
		get {
			if (null == _instance) {
				_instance = new AvatarCreateQueue ();
			}
			return _instance;
		}
	}
	private AvatarCreateQueue () { }

	private bool isEnableCreate = true;
	public Coroutine createCoroutine = null;
	public Queue<AvatarData> createAvatarQueue = new Queue<AvatarData> ();

	//创建
	public void CreateAvatar(RoleEquipmentData equipmentData,SpineAvatarType type,string animationName,bool animationLoop,bool isTexturePackage,Action<Avatar> createSucCallBack = null,Action<bool> wearSucCallBack = null,Action<string> failCallBack = null){
		AvatarData data = new AvatarData();
		data.spineAvatarType = type;
		if(null == equipmentData){
			data.plastomerId = "88888888";
		}else{
			data.plastomerId = equipmentData.RolePlastomerId;
		}
		data.avatarName = type.ToString();
		data.avatarSkinName = SpineConstValue.defaultSkinName;
		data.animationName = animationName;
		data.animationLoop = animationLoop;
		data.isTexturePackage = isTexturePackage;
		data.equipmentData = equipmentData;
		data.createSucCallBack = createSucCallBack;
		data.wearSucCallBack = wearSucCallBack;
		data.failCallBack = failCallBack;
		CreateAvatar(data);
	}
	private void CreateAvatar (AvatarData data) {
		createAvatarQueue.Enqueue (data);
		if (null == createCoroutine) {
			createCoroutine = GameManager.Instance.StartCoroutine (CreateIEnumerator ());
		}
	}
	private IEnumerator CreateIEnumerator () {
		while (createAvatarQueue.Count > 0) {
			isEnableCreate = false;
			AvatarData data = createAvatarQueue.Peek ();
			AvatarManager.Instance.StartCreateAvatar (data, SetEnableCreateCallBack);
			while (!isEnableCreate) {
				yield return null;
			}
			yield return null;
		}
		createCoroutine = null;
	}
	private void SetEnableCreateCallBack (bool isEnable) {
		if(createAvatarQueue.Count == 0) return;
		AvatarData data = createAvatarQueue.Dequeue ();
		isEnableCreate = isEnable;
		if(null != data.wearSucCallBack){
			data.wearSucCallBack.Invoke(isEnable);
		}
	}
	public void ClearCreateQueue(){
		AvatarCreateQueue.Instance.createAvatarQueue.Clear();
		if(null != AvatarCreateQueue.Instance.createCoroutine){
			GameManager.Instance.StopCoroutine(AvatarCreateQueue.Instance.createCoroutine);
			AvatarCreateQueue.Instance.createCoroutine = null;
		}
	}
}