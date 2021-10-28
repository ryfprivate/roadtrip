using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/Save")]
    public class SaveEventChannelSO : BaseDescriptionSO
    {
        public UnityAction<UnityAction<SaveManager>> OnUseManager;
        public void UseManager(UnityAction<SaveManager> callback)
        {
            OnUseManager?.Invoke(callback);
        }
    }
}