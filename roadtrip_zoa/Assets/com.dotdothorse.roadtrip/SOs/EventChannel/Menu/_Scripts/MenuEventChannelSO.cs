using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
 
namespace com.dotdothorse.roadtrip
{
    [CreateAssetMenu(menuName = "SOs/Event Channel/Menu")]
    public class MenuEventChannelSO : BaseDescriptionSO
    {
        public UnityAction<Tab> OnStartMenu;
        public UnityAction OnStartGame;

        public void StartMenu(Tab tab)
        {
            OnStartMenu?.Invoke(tab);
        }

        public void StartGame()
        {
            OnStartGame?.Invoke();
        }
    }
}