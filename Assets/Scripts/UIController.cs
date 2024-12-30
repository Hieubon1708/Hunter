using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Hunter
{
    public class UIController : MonoBehaviour
    {
        public static UIController instance;

        public GamePlay gamePlay;

        public CinemachineVirtualCamera cam;
        private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
        public Animation glow;
        public Animation camAni;
        public float defaultFieldOfView = 90;
        public Image layerCover;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            cinemachineBasicMultiChannelPerlin = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        public void Lose()
        {
            gamePlay.layerCover.SetActive(true);
            gamePlay.buttonReplay.SetActive(true);
        }

        public void Win()
        {
            PlayerPrefs.SetInt("HunterLevel", PlayerPrefs.GetInt("HunterLevel", 1) + 1);
            gamePlay.layerCover.SetActive(true);
        }

        public void LoadUI()
        {
            gamePlay.UpdateRemainingEnemy();
        }

        public void ChangeMap()
        {
            layerCover.raycastTarget = true;
            layerCover.DOFade(1f, 0.5f).OnComplete(delegate
            {
                gamePlay.Restart();
                GameController.instance.LoadLevel(PlayerPrefs.GetInt("HunterLevel", 1));
                layerCover.DOFade(0f, 0.5f).OnComplete(delegate
                {
                    layerCover.raycastTarget = false;

                });
            });
        }

        public void BossEnd()
        {

        }

        public void HitEffect()
        {
            ResetHitEffect();
            glow.Play();
        }

        public void ShakeCam()
        {
            ResetShake();
            Invoke("StartShakeCam", 0.35f);
        }

        public void ShakeCancel()
        {
            CancelInvoke("StartShakeCam");
        }

        public void HitCancel()
        {
            glow.Stop();
        }

        void ResetShake()
        {
            CancelInvoke("StartShakeCam");
            CancelInvoke("StopShakeCam");
            StopShakeCam();
        }

        void ResetHitEffect()
        {
            glow.Stop();
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