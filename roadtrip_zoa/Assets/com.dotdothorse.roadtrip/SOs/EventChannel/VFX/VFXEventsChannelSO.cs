using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/VFX")]
    public class VFXEventsChannelSO : BaseDescriptionSO
    {
        public UnityAction<VFXData> OnRequestCreatePool;
        public UnityAction<string, Transform, UnityAction<Effect>> OnRequestEffect;

        public void RequestCreatePool(VFXData data)
        {
            if (OnRequestCreatePool != null)
            {
                OnRequestCreatePool.Invoke(data);
            } else
            {
                Debug.Log("No VFX player");
            }
        }
        public void RequestEffect(string effectName, Transform parent, UnityAction<Effect> callback)
        {
            if (OnRequestEffect != null)
            {
                OnRequestEffect.Invoke(effectName, parent, callback);
            }
        }
    }
}