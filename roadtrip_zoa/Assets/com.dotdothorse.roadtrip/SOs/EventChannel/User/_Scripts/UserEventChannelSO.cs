using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/User")]
    public class UserEventChannelSO : BaseDescriptionSO
    {
        public UnityAction<UnityAction<UserInfo>> OnAccessUserInfo;
        public void AccessUserInfo(UnityAction<UserInfo> callback)
        {
            OnAccessUserInfo?.Invoke(callback);
        }
    }
}