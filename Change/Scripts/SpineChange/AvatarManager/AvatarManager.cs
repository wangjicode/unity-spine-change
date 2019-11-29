using System;
using System.Collections;
using System.Collections.Generic;
using DancingWord.Resources;
using DancingWord.SpineChange;
using UnityEngine;

public class AvatarManager {
	private AvatarManager () { }
	private static AvatarManager _instance;
	public static AvatarManager Instance {
		get {
			if (null == _instance) {
				_instance = new AvatarManager ();
			}
			return _instance;
		}
	}
	//创建Avatar
	private void CreateAvatar (SpineAvatarType avatarType, string equipmentId, string name, string skinName, string animationName, bool loop, RoleEquipmentData data, Action<Avatar> createSucCallBack, Action<string> failCallBack, Action<bool> createQueueCallBack, bool isTP) {
		SpineChangeManager.Instance.SpineCreateRole (avatarType, RoleEquipmentDataTools.CreateRoleEquipmentItemById (equipmentId), SpineChangeDataLogic.Instance.GetEquipmentMaterial (), true, name, skinName, animationName, loop, data, createSucCallBack, failCallBack, createQueueCallBack, isTP);
	}
	public void StartCreateAvatar (AvatarData avatardata, Action<bool> createQueueCallBack) {
		CreateAvatar (avatardata.spineAvatarType, avatardata.plastomerId, avatardata.avatarName, avatardata.avatarSkinName, avatardata.animationName, avatardata.animationLoop, avatardata.equipmentData, avatardata.createSucCallBack, avatardata.failCallBack, createQueueCallBack, avatardata.isTexturePackage);
	}
	//创建 翅膀、悬浮、手持等。。。
	//TODO
}