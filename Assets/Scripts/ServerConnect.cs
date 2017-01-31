using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Assets.Model;
using UnityEngine;

namespace Assets.Scripts
{
    public class ServerConnect : MonoBehaviour
    {
        // Use this for initialization
        private void Start()
        {
            new Thread(() => C2SRequest(Constants.JOIN)).Start();
        }

        // Update is called once per frame
        private void Update()
        {
            // Ignore
        }

        public void C2SRequest(string message)
        {
            try
            {
                Debug.Log("Creating TCP Client...");
                using (var client = new TcpClient(Constants.SERVER_IP, Constants.SERVER_PORT))
                {
                    Debug.Log("Created TCP Client");
                    var byteData = Encoding.ASCII.GetBytes(message);
                    Debug.Log("Writing to Stream...");
                    client.GetStream().Write(byteData, 0, byteData.Length);
                    Debug.Log("Written to Stream");
                    Debug.Log("Closing TCP Client");
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}