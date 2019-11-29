using System;
using System.Collections;
using System.Collections.Generic;
using DancingWord.Data;
using DancingWord.Net;
using LitJson;
using UnityEngine;

namespace DancingWord.SpineChange {
    public class RoleEquipmentDataTools {
        //根据RoleEquipmentData创建NetRoleEquipmentData
        public static NetRoleEquipmentData RoleEquipmentDataToNetData (RoleEquipmentData data) {
            NetRoleEquipmentData netData = new NetRoleEquipmentData ();
            netData.plastomer_id = int.Parse (data.RolePlastomerId);
            netData.equip_list = new NetEquipmentItem[data.RoleEquipmentItems.Count];
            for (int i = 0; i < data.RoleEquipmentItems.Count; i++) {
                netData.equip_list[i] = new NetEquipmentItem ();
                netData.equip_list[i].item_id = int.Parse ((((int) data.RoleEquipmentItems[i].EquipmentType) / 10).ToString () + data.RoleEquipmentItems[i].EquipmentId);
                netData.equip_list[i].item_type = ((int) data.RoleEquipmentItems[i].EquipmentType);
            }
            return netData;
        }
        //根据NetRoleEquipmentData创建RoleEquipmentData
        public static RoleEquipmentData NetDataToRoleEquipmentData (NetRoleEquipmentData data) {
            RoleEquipmentData roleData = new RoleEquipmentData ();
            roleData.RolePlastomerId = data.plastomer_id.ToString ();
            roleData.RoleEquipmentItems = new List<RoleEquipmentItem> ();
            for (int i = 0; i < data.equip_list.Length; i++) {
                roleData.RoleEquipmentItems.Add (CreateRoleEquipmentItemById (data.equip_list[i].item_id.ToString ()));
            }
            return roleData;
        }
        //通过装备的ID创建RoleEquipmentItem
        public static RoleEquipmentItem CreateRoleEquipmentItemById (string id) {
            if (id.Length < 8) {
                throw new ArgumentException ("装备Id长度不足8位！");
            }
            if (SpineConstValue.splitIdStartIndex >= id.Length ||
                SpineConstValue.splitIdLength > id.Length ||
                SpineConstValue.splitTypeLength > id.Length ||
                SpineConstValue.splitIdStartIndex + SpineConstValue.splitIdLength > id.Length) {
                throw new ArgumentException ("SpineConstValue.splitIdLength || SpineConstValue.splitTypeLength || SpineConstValue.splitTypeLength 不合法！");
            }
            int partType = int.Parse (id.Substring (0, SpineConstValue.splitTypeLength));
            int partId = int.Parse (id.Substring (SpineConstValue.splitIdStartIndex, SpineConstValue.splitIdLength));
            return new RoleEquipmentItem { EquipmentId = partId, EquipmentType = (RoleEquipmentType) partType };
        }
        //通过RoleEquipmentItem创建装备ID
        public static string CreateIdByRoleEquipmentItem (RoleEquipmentItem item) {
            return string.Concat (((int) item.EquipmentType / 10).ToString (), item.EquipmentId.ToString ());
        }
        //更换装备存数据
        public static void SaveSelfEquipmentData(string equipmentId){
            //Debug.Log(JsonMapper.ToJson(PlayerDataManager.Instance().SelfPlayer.equipmentData));
            SpineChangeDataLogic.Instance.SaveRoleEquipmentToServerAndLocal(PlayerDataManager.Instance().SelfPlayer.equipmentData,CreateRoleEquipmentItemById(equipmentId));
            //Debug.Log(JsonMapper.ToJson(PlayerDataManager.Instance().SelfPlayer.equipmentData));
        }

        public static bool IsContainsEquipmentId(string id){
            List<RoleEquipmentItem> tempList = PlayerDataManager.Instance().SelfPlayer.equipmentData.RoleEquipmentItems;
            for(int i = 0;i<tempList.Count;i++) {
                if(id.Equals(CreateIdByRoleEquipmentItem(tempList[i]))){
                    return true;
                }
            }
            return false;
        }
    }
}