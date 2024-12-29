using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public GamePlay gamePlay;

        public CinemachineVirtualCamera cam;
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
        public Animation glow;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            cinemachineBasicMultiChannelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void LoadUI()
        {
            gamePlay.UpdateRemainingEnemy();
        }

        public void HitEffect()
        {
            ResetHitEffect();
            glow.Play();
            Invoke("StartShakeCam", 0.35f);
        }

        public void HitCancel()
        {
            glow.Stop();
            CancelInvoke("StartShakeCam");
        }

        void ResetHitEffect()
        {
            glow.Stop();
            CancelInvoke("StartShakeCam");
            CancelInvoke("StopShakeCam");
            StopShakeCam();
        }

        void StartShakeCam()
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5f;
            Invoke("StopShakeCam", 0.25f);
        }

        void StopShakeCam()
        {
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
        }
    }
}