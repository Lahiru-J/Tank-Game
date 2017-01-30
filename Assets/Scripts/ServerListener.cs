using System;
using System.Collections.Generic;
using System.IO;
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
        private Game _gamedata;
        public int Xsize;
        public int Ysize;

        // Use this for initialization
        private void Start()
        {
            _gamedata = new Game(new Grid(Xsize, Ysize));
            var thread = new Thread(Listen);
            thread.Start();
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
    }
}