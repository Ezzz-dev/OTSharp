/**
 * OTSharp 7.6 - a free and open-source MMORPG server emulator
 * Copyright (C) 2014  Daniel Alejandro <alejandrodemujica@gmail.com>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using GameServer.Network;

namespace GameServer.Protocol
{
    /// <summary>
    /// Class used to manage incoming connections
    /// </summary>
    public class ProtocolGame
    {
        #region Initializator

        /// <summary>
        /// Initialize new protocol service base
        /// </summary>
        /// <param name="toListenAddress"></param>
        /// <param name="toListenPort"></param>
        public ProtocolGame(string toListenAddress, int toListenPort)
        {
            Listener = new TcpListener(IPAddress.Parse(toListenAddress), toListenPort);
            ListenIPAddress = toListenAddress;
            ListenPort = toListenPort;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Thread which listens for incoming connections
        /// This thread is not running on background!
        /// </summary>
        public Thread ListenThread { get; set; }
        /// <summary>
        /// IP Address the service is listening to
        /// </summary>
        public string ListenIPAddress { get; set; }
        /// <summary>
        /// Port the service is using to listen
        /// </summary>
        public int ListenPort { get; set; }
        /// <summary>
        /// TCP Listener
        /// </summary>
        public TcpListener Listener { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Start listening for connections
        /// If this method fails to start, no connections will be edible to listen
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                Listener.Start();
                ListenThread = new Thread(new ThreadStart(Listen));
                ListenThread.Start();
            }
            catch (SocketException exception)
            {
                switch ((SocketError)exception.ErrorCode)
                {
                    case SocketError.AddressAlreadyInUse:
                        Console.WriteLine("ProtocolService: could not bind {0}:{1}, already in use.", ListenIPAddress, ListenPort);
                        break;
                    case SocketError.AddressNotAvailable:
                        Console.WriteLine("ProtocolService: invalid IP address.");
                        break;
                    default:
                        Console.WriteLine("ProtocolService: {0}", exception.ToString());
                        break;
                }
                return false;
            }
            catch (ThreadStateException)
            {
                Console.WriteLine("ProtocolService: Listen thread has already been started. Overflow.");
                return false;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine("ProtocolService: Out of memory to start thread.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Loop function which listens for TCP connections
        /// </summary>
        private void Listen()
        {
            while (true)
            {
                Listener.BeginAcceptSocket(new AsyncCallback(onConnectionReceive), null);
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// Method called when a connection is received
        /// </summary>
        /// <param name="result"></param>
        private void onConnectionReceive(IAsyncResult result)
        {
            Socket incomingSocket = Listener.EndAcceptSocket(result);
            Connection connection = new Connection(incomingSocket);
            connection.onConnectionInit();
        }

        #endregion
    }
}
