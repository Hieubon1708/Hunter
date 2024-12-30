using Cinemachine;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Hunter
{
    public class GameController : MonoBehaviour
    {
        public static GameController instance;

        public GameObject preNormalBot;
        public GameObject preSniperBot;
        public GameObject preBossBot;

        public Transform poolEnemy;
        public Transform poolPoppy;
        public Transform poolWeapon;

        public Camera cam;
        public WeaponEquip weaponEquip;

        public List<Bot> bots;
        public List<Bot> botsReserve;
        public PathInfo[] pathInfos;

        public bool isResarting;

        public List<PoppyType> poppyTypes;
        public List<Player> poppiesReserve;
        public List<Player> poppies;
        public GameObject[] prePoppies;

        public CinemachineVirtualCamera camFollow;
        GameObject map;

        public void Awake()
        {
            instance = this;
        }

        public PoppyType RandomPoppy()
        {
            int indexRandom = Random.Range(0, 8);
            switch (indexRandom)
            {
                case 0: return PoppyType.Bobby;
                case 1: return PoppyType.Bubba;
                case 2: return PoppyType.Poppy;
                case 3: return PoppyType.Catnap;
                case 4: return PoppyType.Craftycorn;
                case 5: return PoppyType.Hoppy;
                case 6: return PoppyType.Kickin;
                case 7: return PoppyType.Pickypiggy;
                default: return PoppyType.Poppy;
            }
        }

        public void Start()
        {
            PlayerPrefs.DeleteAll();
            LoadLevel(PlayerPrefs.GetInt("HunterLevel", 1));
        }

        public enum BotType
        {
            Normal, Sniper, Boss
        }

        public enum PathType
        {
            Repeat, Circle
        }

        public enum WeaponType
        {
            Knife
        }

        public enum PoppyType
        {
            Poppy, Bobby, Bubba, Catnap, Craftycorn, Hoppy, Kickin, Pickypiggy
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
            isResarting = true;
            LoadPoppy();
            LoadWeaponPoppies(WeaponType.Knife);
            for (int i = 0; i < poolEnemy.childCount; i++)
            {
                Destroy(poolEnemy.GetChild(i).gameObject);
            }
            if (map != null) Destroy(map);
            map = Instantiate(Resources.Load<GameObject>(level.ToString()));
            PlayerController.instance.ResetGame();
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].ResetPlayer();
            }
            pathInfos = map.GetComponentsInChildren<PathInfo>();
            poppiesReserve = new List<Player>(poppies);
            botsReserve = new List<Bot>(bots);
            ResetBots();
            if (!IsBoss()) StartBots();
            else StartCoroutine(UIController.instance.BossIntro());
            LoadUI();
            DOVirtual.DelayedCall(0.02f, delegate { isResarting = false; });
        }

        void LoadPoppy()
        {
            for (int i = poppies.Count; i < poppyTypes.Count; i++)
            {
                AddPoppy(poppyTypes[i]);
            }
        }

        public bool IsBoss()
        {
            for (int i = 0; i < pathInfos.Length; i++)
            {
                if (pathInfos[i].botType == BotType.Boss) return true;
            }
            return false;
        }

        public Bot GetBoss()
        {
            for (int i = 0; i < pathInfos.Length; i++)
            {
                if (pathInfos[i].botType == BotType.Boss) return bots[i];
            }
            return null;
        }

        public void RemovePoppy(Player poppy)
        {
            poppies.Remove(poppy);
            if (poppies.Count == 0)
            {
                PlayerController.instance.Lose();
                UIController.instance.Lose();
            }
        }

        void LoadWeaponPoppies(WeaponType weaponType)
        {
            for (int i = poppies.Count - 1; i < poppyTypes.Count; i++)
            {
                poppies[i].LoadWeapon(weaponType);
            }
        }

        public Player GetPoppy(GameObject poppy)
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                if (poppies[i].gameObject == poppy)
                {
                    return poppies[i];
                }
            }
            return null;
        }

        void LoadWeaponPoppy(WeaponType weaponType, Player player)
        {
            player.LoadWeapon(weaponType);
        }

        public Player AddPoppy(PoppyType poppyType)
        {
            GameObject p = Instantiate(prePoppies[(int)poppyType], poolPoppy);
            Player sc = p.GetComponent<Player>();
            poppies.Add(sc);
            return sc;
        }

        void RemovePoppies()
        {
            for (int i = 0; i < poppies.Count; i++)
            {
                if (!poppies[i].gameObject.activeSelf)
                {
                    Destroy(poppies[i].gameObject);
                }
            }
        }

        void LoadUI()
        {
            UIController.instance.LoadUI();
        }

        public Bot GetBot(GameObject bot)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i].gameObject == bot)
                {
                    return bots[i];
                }
            }
            return null;
        }

        public void RemoveBot(GameObject bot)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i].gameObject == bot)
                {
                    bots.RemoveAt(i);
                }
            }
        }

        public void ResetBots()
        {
            for (int i = 0; i < bots.Count; i++)
            {
                bots[i].ResetBot();
            }
        }

        public void SetBot(BotType botType, PathInfo pathInfo)
        {
            if (botType == BotType.Normal)
            {
                bots.Add(Instantiate(preNormalBot, poolEnemy).GetComponent<Bot>());
            }
            if (botType == BotType.Boss)
            {
                bots.Add(Instantiate(preBossBot, poolEnemy).GetComponent<Bot>());
            }
            bots[bots.Count - 1].Init(pathInfo);
        }

        public void StartBots()
        {
            for (int i = 0; i < bots.Count; i++)
            {
                bots[i].StartProbe(1);
            }
        }

        public void Replay()
        {
            bots = new List<Bot>(botsReserve);
            poppies = new List<Player>(poppiesReserve);
            ResetBots();
            StartBots();
            PlayerController.instance.ResetGame();
            for (int i = 0; i < poppies.Count; i++)
            {
                poppies[i].ResetPlayer();
            }
        }
    }
}
