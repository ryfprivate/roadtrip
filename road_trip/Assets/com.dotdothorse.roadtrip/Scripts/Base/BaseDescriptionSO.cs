using UnityEngine;
 
namespace com.dotdothorse.roadtrip
{
    public class BaseDescriptionSO : ScriptableObject
    {
        [TextArea] public string description;
    }
}