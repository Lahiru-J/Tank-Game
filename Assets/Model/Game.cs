using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts;
using UnityEditor.VersionControl;
using UnityEngine;

namespace Assets.Model
{
    public class Game
    {
        private int? _playerNo;
        public List<Cell> Cells;
        public List<CoinPack> Coinpacks;
        public Grid Grid;
        public List<LifePack> Lifepacks;
        public Dictionary<int, Tank> Tanks;

        public Game(Grid grid)
        {
            Grid = grid;
            Cells = new List<Cell>();
            Tanks = new Dictionary<int, Tank>();
            Coinpacks = new List<CoinPack>();
            Lifepacks = new List<LifePack>();
        }

        #region Client Request Execution

        public void ExecuteClientRequest(ClientRequest request)
        {
            switch (request)
            {
                case ClientRequest.Down:
                    new ServerConnect().C2SRequest(Constants.DOWN);
                    break;
                case ClientRequest.Join:
                    new ServerConnect().C2SRequest(Constants.JOIN);
                    break;
                case ClientRequest.Left:
                    new ServerConnect().C2SRequest(Constants.LEFT);
                    break;
                case ClientRequest.Right:
                    new ServerConnect().C2SRequest(Constants.RIGHT);
                    break;
                case ClientRequest.Shoot:
                    new ServerConnect().C2SRequest(Constants.SHOOT);
                    break;
                case ClientRequest.Up:
                    new ServerConnect().C2SRequest(Constants.UP);
                    break;

                default:
                    Debug.LogError("Unidentified Request - " + request);
                    break;
            }
        }
        

     

        #endregion

        #region Server Command Execution

        public void ExecuteServerCommand(string command)
        {
            // Errors
            switch (command.TrimEnd('#'))
            {
                case Constants.S2C_ALREADYADDED:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_CELLOCCUPIED:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_PLAYERSFULL:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_FALLTOPIT:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_JUSTFINISHED:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_GAMEOVER:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_STARTED:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_HITOBSTACLE:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_INVALIDCELL:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_NOTACONTESTANT:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_DEAD:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_NOTSTARTED:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_REQUESTERROR:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_SERVERERROR:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
                case Constants.S2C_TOOQUICK:
                    Debug.Log("Processing Command -> " + command.TrimEnd('#'));
                    break;
            }

            var identifier = command.Substring(0, 2);

            // Broadcasts
            switch (identifier)
            {
                // Game Acceptance
                case "S:":
                    Debug.Log("Identified Command -> " + identifier + command.TrimEnd('#'));
                    GameAcceptanceExec(command.TrimEnd('#'));
                    break;

                // Game Initiation
                case "I:":
                    Debug.Log("Identified Command -> " + identifier + command.TrimEnd('#'));
                    GameInitExec(command.TrimEnd('#'));
                    break;

                // Game Update
                case "G:":
                    Debug.Log("Identified Command -> " + identifier + command.TrimEnd('#'));
                    GameUpdateExec(command.TrimEnd('#'));
                    break;

                // Coin Spawn
                case "C:":
                    Debug.Log("Identified Command -> " + identifier + command.TrimEnd('#'));
                    CoinSpawnExec(command.TrimEnd('#'));
                    break;

                // Lifepack Spawn
                case "L:":
                    Debug.Log("Identified Command -> " + identifier + command.TrimEnd('#'));
                    LifepackSpawnExec(command.TrimEnd('#'));
                    break;

                // Unidentified Message
                default:
                    Debug.LogError("Command Not Identified -> " + command.TrimEnd('#'));
                    break;
            }
        }

        /// <summary>
        ///     FORMAT = [S : Pn;x,y;d : Pn;x,y;d#]
        /// </summary>
        /// <param name="command">Command.</param>
        private void GameAcceptanceExec(string command)
        {
            try
            {
                var positions = command.Substring(2).Split(':');

                foreach (var position in positions)
                {
                    var fields = position.Split(';');
                    var number = int.Parse(fields[0].Substring(1));
                    Tanks[number] = new Tank(number, number == _playerNo, int.Parse(fields[1].Split(',')[0]),
                        int.Parse(fields[1].Split(',')[1]), int.Parse(fields[2]));
                }

                // Generate All Tanks
                GenerateGameObjects.GetInstance().GenerateTanks(Tanks);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        ///     FORMAT = [I : Pn : x,y; ... ;x,y : x,y; ... ;x,y : x,y; ... ;x,y#]
        /// </summary>
        /// <param name="command">Command.</param>
        private void GameInitExec(string command)
        {
            try
            {
                var data = command.Substring(2).Split(':');

                // Set player number variable (Does not update tanks dictionary)
                _playerNo = int.Parse(data[0].Substring(1));

                // Cells Initiation
                Cells.Clear();
                for (var i = 1; i <= 3; i++)
                    foreach (var coordinatePair in data[i].Split(';'))
                    {
                        var coordinates = coordinatePair.Split(',');
                        Cells.Add(new Cell(int.Parse(coordinates[0]), int.Parse(coordinates[1]), i - 1, 0));
                    }

                // Generate All Cells
                GenerateGameObjects.GetInstance().GenerateCells(Cells);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        ///     FORMAT = [G : Pn;x,y;d;shot;health;coins;points : ... : Pn;x,y;d;shot;health;coins;points : x,y,damage; ...
        ///     ;x,y,damage#]
        /// </summary>
        /// <param name="command">Command.</param>
        private void GameUpdateExec(string command)
        {
            try
            {
                var data = command.Substring(2).Split(':');

                foreach (var d in data)
                    if (d[0] == 'P')
                    {
                        // Tank Status Update [Pn;x,y;d;shot;health;coins;points]
                        var state = d.Split(';');
                        var currentTank = Tanks[int.Parse(state[0].Substring(1))];
                        currentTank.X = int.Parse(state[1].Split(',')[0]);
                        currentTank.Y = int.Parse(state[1].Split(',')[1]);
                        currentTank.Direction = (Direction) int.Parse(state[2]);
                        currentTank.IsShot = int.Parse(state[3]) == 1;
                        currentTank.Health = int.Parse(state[4]);
                        currentTank.Coins = int.Parse(state[5]);
                        currentTank.Points = int.Parse(state[6]);
                    }
                    else
                    {
                        // Cell status update [x,y,damage ; ... ; x,y,damage]
                        foreach (var locationupdate in d.Split(';'))
                        {
                            var info = locationupdate.Split(',');
                            var cell =
                                Cells.FirstOrDefault(c => (c.X == int.Parse(info[0])) && (c.Y == int.Parse(info[1])));
                            if (cell != null)
                                cell.Damage = (CellDamage) int.Parse(info[2]);
                        }
                    }

                // Generate all Tanks and Cells
                GenerateGameObjects.GetInstance().GenerateCells(Cells);
                GenerateGameObjects.GetInstance().GenerateTanks(Tanks);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        ///     FORMAT = [C:x,y:lt:val#]
        /// </summary>
        /// <param name="command">Command.</param>
        private void CoinSpawnExec(string command)
        {
            try
            {
                var coins = command.Substring(2).Split(':');
                var location = coins[0].Split(',');
                var coinPack = new CoinPack(int.Parse(location[0]), int.Parse(location[1]), int.Parse(coins[1]),
                    int.Parse(coins[2]));
                // Generate Coin Packs
                GenerateGameObjects.GetInstance().GenerateCoinPacks(coinPack);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        ///     FORMAT = [L:x,y:lt#]
        /// </summary>
        /// <param name="command">Command.</param>
        private void LifepackSpawnExec(string command)
        {
            try
            {
                var lifes = command.Substring(2).Split(':');
                var location = lifes[0].Split(',');
                var lifePack = new LifePack(int.Parse(location[0]), int.Parse(location[1]), int.Parse(lifes[1]));

                // Generate Life Packs
                GenerateGameObjects.GetInstance().GenerateLifePacks(lifePack);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion
    }
}