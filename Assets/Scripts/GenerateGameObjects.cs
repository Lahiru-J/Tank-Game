using System.Collections.Generic;
using Assets.Model;
using UnityEngine;

namespace Assets.Scripts
{
    internal class GenerateGameObjects : MonoBehaviour
    {
        private const int XFactor = 1;
        private const int YFactor = 1;
        private const int ZPos = 0;

        private static GenerateGameObjects _instance;
        private static readonly Quaternion OriginalRot = Quaternion.identity;
        public GameObject Brick;
        public GameObject CoinPack;
        public GameObject EnemyTank;
        public GameObject LifePack;
        public GameObject PlayerTank;
        public GameObject Stone;
        public GameObject Water;

        public static GenerateGameObjects GetInstance()
        {
            return _instance;
        }

        private void Start()
        {
            _instance = this;
        }

        public void GenerateCells(List<Cell> cellList)
        {
            GameObject prototype = null;

            foreach (var cell in cellList)
            {
                if (cell.Type == CellType.Brick) prototype = Brick;
                if (cell.Type == CellType.Water) prototype = Water;
                if (cell.Type == CellType.Stone) prototype = Stone;
                UpdateGameObject(cell.ToString(), prototype, new Vector3(cell.X*XFactor, cell.Y*YFactor, ZPos), OriginalRot);
            }
        }

        public void GenerateTanks(Dictionary<int, Tank> tanks)
        {
            foreach (var tank in tanks)
            {
                var rotation = 0;

                switch (tank.Value.Direction)
                {
                    case Direction.North:
                        rotation = 180;
                        break;
                    case Direction.East:
                        rotation = -90;
                        break;
                    case Direction.South:
                        rotation = 0;
                        break;
                    case Direction.West:
                        rotation = 90;
                        break;
                }

                UpdateGameObject(tank.ToString(), tank.Value.IsPlayer ? PlayerTank : EnemyTank,
                    new Vector3(tank.Value.X, tank.Value.Y, ZPos),
                    Quaternion.Euler(0, 0, rotation));
            }
        }

        public void GenerateCoinPacks(CoinPack coinPack)
        {
            var obj = UpdateGameObject(coinPack.ToString(), CoinPack, new Vector3(coinPack.X, coinPack.Y, ZPos), OriginalRot);
            Destroy(obj, (float) coinPack.Lifetime/1000);
        }

        public void GenerateCoinPacks(List<CoinPack> coinPacks)
        {
            foreach (var coinPack in coinPacks)
            {
                var obj = UpdateGameObject(coinPack.ToString(), CoinPack, new Vector3(coinPack.X, coinPack.Y, ZPos),
                    OriginalRot);
                Destroy(obj, (float) coinPack.Lifetime/1000);
            }
        }

        public void GenerateLifePacks(LifePack lifePack)
        {
            var obj = UpdateGameObject(lifePack.ToString(), LifePack, new Vector3(lifePack.X, lifePack.Y, ZPos), OriginalRot);
            Destroy(obj, (float) lifePack.Lifetime/1000);
        }

        public void GenerateLifePacks(List<LifePack> lifePacks)
        {
            foreach (var lifePack in lifePacks)
            {
                var obj = UpdateGameObject(lifePack.ToString(), LifePack, new Vector3(lifePack.X, lifePack.Y, ZPos),
                    OriginalRot);
                Destroy(obj, (float) lifePack.Lifetime/1000);
            }
        }

        private GameObject UpdateGameObject(string name, GameObject gameObj, Vector3 pos, Quaternion rot)
        {
            var obj = GameObject.Find(name);
            if (obj == null)
            {
                obj = Instantiate(gameObj, pos, rot);
                obj.name = name;
            }
            else
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;
            }
            return obj;
        }
    }
}