using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _followCamera;
    public void StartView(GameObject player)
    {
        SetLookAtTarget(player);
        SetFollowTarget(player);
        var transposer = _followCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = new Vector3(0, 2, -5);
    }
    public void GameView()
    {
        var transposer = _followCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 offset = transposer.m_FollowOffset;

        DOTween.To(() => offset, x => offset = x, new Vector3(0, 4, -10), 2)
            .OnUpdate(() =>
            {
                transposer.m_FollowOffset = offset;
            });
    }
    private void SetLookAtTarget(GameObject player)
    {
        _followCamera.LookAt = player.transform;
    }
    private void SetFollowTarget(GameObject player)
    {
        _followCamera.Follow = player.transform;
    }
    public void UnfollowTargets()
    {
        _followCamera.Follow = null;
    }
}
