using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public List<NormalBot> poolNormalBots;
        public List<SniperBot> poolSniperBots;
        public List<BossBot> poolBoosBots;

        public int indexNormalBot;
        public int indexSniperBot;
        public int indexBoosBot;

        public GameObject preNormalBot;
        public GameObject preSniperBot;
        public GameObject preBossBot;

        public Transform container;

        public Camera cam;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            LoadLevel(1);
        }

        public enum BotType
        {
            Normal, Sniper, Boss
        }
        
        public enum PathType
        {
            Repeat, Circle
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                // do something
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                // do something
            }
        }

        public void LoadLevel(int level)
        {
            ResetGame();
            GameObject map = Instantiate(Resources.Load<GameObject>(level.ToString()));
        }

        public void Replay()
        {

        }

        void ResetBots()
        {
            for (int i = 0; i < poolNormalBots.Count; i++)
            {
                poolNormalBots[i].ResetBot();
            }
            for (int i = 0; i < poolSniperBots.Count; i++)
            {
                poolSniperBots[i].ResetBot();
            }
            for (int i = 0; i < poolBoosBots.Count; i++)
            {
                poolBoosBots[i].ResetBot();
            }
        }


        public void SetBot(BotType botType, PathInfo pathInfo)
        {
            if (botType == BotType.Normal)
            {
                if (indexNormalBot == poolNormalBots.Count)
                {
                    poolNormalBots.Add(Instantiate(preNormalBot, container).GetComponent<NormalBot>());
                }
                poolNormalBots[indexNormalBot].Init(pathInfo);
                indexNormalBot++;
            }
        }

        public void ResetGame()
        {
            indexNormalBot = 0;
            indexSniperBot = 0;
            indexBoosBot = 0;
        }
    }
}
