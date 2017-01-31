using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Assets.Model;
using UnityEngine;

namespace Assets.Scripts
{
    public class ServerListener : MonoBehaviour
    {
        private readonly object _commandLock = new object();

        private readonly Queue<string> _commandQueue = new Queue<string>();
        // Stores all required gamedata for the current session
        private static Game _gamedata;
        public int Xsize;
        public int Ysize;

        // Use this for initialization
        private void Start()
        {
            Debug.Log("Starting server listener...");
            _gamedata = new Game(new Grid(Xsize, Ysize));
            var thread = new Thread(Listen);
            //var aiThread = new Thread(active);
            thread.Start();
            //aiThread.Start();
        }

        // Update is called once per frame
        private void Update()
        {
            if (_commandQueue.Count > 0)
            {
                string command;
                lock (_commandLock)
                {
                    command = _commandQueue.Dequeue();
                }
                _gamedata.ExecuteServerCommand(command);
            }
        }

        private void Listen()
        {
            try
            {
                var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7000);
                Debug.Log("Starting TCP Listener...");
                tcpListener.Start();
                Debug.Log("Started TCP Listener");
                var listen = true;
                var result = string.Empty;

                while (listen)
                {
                    Debug.Log("Opening Network Stream...");
                    using (var networkStream = tcpListener.AcceptTcpClient().GetStream())
                    {
                        Debug.Log("Opened Network Stream");
                        var reader = new StreamReader(networkStream);
                        var data = reader.ReadToEnd();
                        networkStream.Flush();
                        Debug.Log("Received Data -> " + data);
                        result += data;
                        Debug.Log("Closing Network Stream...");
                    }
                    Debug.Log("Closed Network Stream");

                    // Make use of the string
                    while (result.Contains("#"))
                    {
                        // extract single command
                        var command = result.Substring(0, result.IndexOf('#') + 1);
                        Debug.Log("Command Identified -> " + command);

                        Debug.Log("Adding command to Queue...");
                        lock (_commandLock)
                        {
                            _commandQueue.Enqueue(command);
                        }
                        Debug.Log("Added command to Queue");
                        result = result.Remove(0, command.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #region AI

        public static List<Tank> TankList;
        public static List<Cell> BrickList;
        public static List<Cell> WaterList;
        public static List<Cell> StoneList;
        public static List<CoinPack> CoinList;
        public static List<LifePack> LifeList;
        public static string[][] Map = new string[10][] { new string[10], new string[10], new string[10], new string[10], new string[10], new string[10], new string[10], new string[10], new string[10], new string[10] };
        public static int[][] Distance = new int[10][] { new int[10], new int[10], new int[10], new int[10], new int[10], new int[10], new int[10], new int[10], new int[10], new int[10] };
        public static Vector3 PlayerTankPossition;
        public static Vector3 coin;
        public static int PreNum;
        public static Boolean preNumOk;

        public static void active()
        {
            //while (!Controller.inited) { }

            Debug.Log("*****************************Starting active thread...");
            while (true)
            {
                Debug.Log("inside while loop*************");
                Thread.Sleep(1000);
                // HQ.shoot();
                SetMap();
                TankList = _gamedata.Tanks.Values.ToList();
                BrickList = _gamedata.Cells.Where(c => c.Type == CellType.Brick).ToList();
                WaterList = _gamedata.Cells.Where(c => c.Type == CellType.Water).ToList();
                StoneList = _gamedata.Cells.Where(c => c.Type == CellType.Stone).ToList();
                CoinList = _gamedata.Coinpacks;
                LifeList = _gamedata.Lifepacks;
                Debug.Log("************ before if ");
                if (TankList.Count > 0)
                {
                    Debug.Log("inside if condition*************");
                    SetCoinsBricks();
                    InitDistance();
                    foreach (Tank tank in TankList) { if (tank.IsPlayer) { PlayerTankPossition = tank.Possition; } }
                    FindCoin();
                    if (coin != PlayerTankPossition)
                    {
                        Distance[(int)coin.x][(int)coin.y] = 0;
                        Setdistance2();
                        Proceed();
                    }
                }
                Debug.Log("************ end if ");

            }

        }

        public static void FindCoin()
        {
            float distance = 10000;
            coin = PlayerTankPossition;
            foreach (CoinPack c in CoinList)
            {
                if (c.IsAvailable && distance > Vector3.Distance(c.Possition, PlayerTankPossition))
                {
                    distance = Vector3.Distance(c.Possition, PlayerTankPossition);
                    coin = c.Possition;
                    Debug.Log("Coin Found At" + coin.x + "," + coin.y);

                }
            }
        }
        public static void SetMap()
        {
            Debug.Log("setting up map*************");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Map[i][j] = "p";

                }
            }
            foreach (Cell s in StoneList)
            {
                Map[s.X][s.Y] = "o";
            }
            foreach (Cell w in WaterList)
            {
                Map[w.X][w.Y] = "o";
            }

        }
        public static void SetCoinsBricks()
        {
            Debug.Log("setting up coins*************");
            foreach (CoinPack c in CoinList)
            {
                Map[c.X][c.Y] = "c";
            }
            foreach (Cell b in BrickList)
            {
                Map[b.X][b.Y] = "p";
                if (!b.Damage.Equals(CellDamage.Percent100))
                {
                    Map[b.X][b.Y] = "o";
                }

            }
        }
        public static void setDistance()
        {

        }
        public static void Setdistance2()
        {

            int dist = 0;
            Boolean tankfound = false;
            int tankX = (int)PlayerTankPossition.x;
            int tankY = (int)PlayerTankPossition.y;

            while (dist < 25)
            {
                Debug.Log("Current distance is: " + dist);
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {

                        if (Distance[i][j] == dist)
                        {

                            if (i < 9 && Distance[i + 1][j] > 0)
                            {
                                Debug.Log("changing distance at " + (i + 1) + " " + j + "tank at" + tankX + "" + tankY);
                                Distance[i + 1][j] = Math.Min(dist + 1, Distance[i + 1][j]);
                                if (i == tankX && j == tankY)
                                {
                                    Debug.Log("******Tank found");
                                    tankfound = true;
                                }
                            }
                            if (j < 9 && Distance[i][j + 1] > 0)
                            {
                                Debug.Log("changing distance at " + i + " " + (j + 1) + "tank at" + tankX + "" + tankY);
                                Distance[i][j + 1] = Math.Min(dist + 1, Distance[i][j + 1]);
                                if (i == tankX && j == tankY)
                                {
                                    Debug.Log("******Tank found");
                                    tankfound = true;
                                }
                            }
                            if (i > 0 && Distance[i - 1][j] > 0)
                            {
                                Debug.Log("changing distance at " + (i - 1) + " " + j + "tank at" + tankX + "" + tankY);
                                Distance[i - 1][j] = Math.Min(dist + 1, Distance[i - 1][j]);
                                if (i == tankX && j == tankY)
                                {
                                    Debug.Log("******Tank found");
                                    tankfound = true;
                                }
                            }
                            if (j > 0 && Distance[i][j - 1] > 0)
                            {
                                Debug.Log("changing distance at " + i + " " + (j - 1) + "tank at" + tankX + "" + tankY);
                                Distance[i][j - 1] = Math.Min(dist + 1, Distance[i][j - 1]);
                                if (i == tankX && j == tankY)
                                {
                                    Debug.Log("******Tank found");
                                    tankfound = true;
                                }
                            }
                            if (tankfound) break;
                        }
                    }
                    if (tankfound) break;
                }
                if (tankfound) break;
                dist++;
            }
        }
        public static void InitDistance()
        {
            Debug.Log("initiating distance*************");
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; ++j)
                {
                    if (Map[i][j] == "o") { Distance[i][j] = -1000; }
                    else Distance[i][j] = 100;

                }
            }
        }
        public static void Proceed()
        {
            Debug.Log("proceeding*************");
            int tnkX = (int)PlayerTankPossition.x;
            int tnkY = (int)PlayerTankPossition.y;
            int left = 100;
            if (tnkX > 0 && Distance[tnkX - 1][tnkY] > -1)
            {
                left = Distance[tnkX - 1][tnkY];
            }
            int right = 100;
            if (tnkX < 9 && Distance[tnkX + 1][tnkY] > -1)
            {
                right = Distance[tnkX + 1][tnkY];
            }
            int top = 100;
            if (tnkY < 9 && Distance[tnkX][tnkY + 1] > -1)
            {
                top = Distance[tnkX][tnkY + 1];
            }
            int bottom = 100;
            if (tnkY > 0 && Distance[tnkX][tnkY - 1] > -1)
            {
                bottom = Distance[tnkX][tnkY - 1];
            }
            int min = Math.Min(Math.Min(left, right), Math.Min(top, bottom));
            if (min == right) { _gamedata.ExecuteClientRequest(ClientRequest.Right); }
            else if (min == left) { _gamedata.ExecuteClientRequest(ClientRequest.Left); }
            else if (min == top) { _gamedata.ExecuteClientRequest(ClientRequest.Up); }
            else if (min == bottom) { _gamedata.ExecuteClientRequest(ClientRequest.Down); }
        }

        #endregion
    }
}