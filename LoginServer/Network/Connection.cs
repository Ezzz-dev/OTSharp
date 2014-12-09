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

namespace LoginServer.Network
{
    /// <summary>
    /// Manage an active TCP connection
    /// </summary>
    public class Connection
    {
        #region Initializator

        /// <summary>
        /// Initialize a new connection with the given socket
        /// </summary>
        /// <param name="activeSocket"></param>
        public Connection(Socket activeSocket)
        {
            ConnectionSocket = activeSocket;
            Stream = new NetworkStream(ConnectionSocket);
        }

        #endregion

        #region Properties

        private Socket ConnectionSocket { get; set; }
        private NetworkStream Stream { get; set; }

        #endregion

        #region Functioning

        /// <summary>
        /// Close this connection network
        /// </summary>
        public void Disconnect()
        {
            Stream.Close();
            ConnectionSocket.Close();
        }

        /// <summary>
        /// Write a network packet to the active connection socket
        /// </summary>
        /// <param name="msg"></param>
        public void Send(NetworkMessage msg)
        {
            msg.AddPacketSize();
            Stream.Write(msg.Buffer, 0, msg.Length);
        }

        #endregion

        #region Connection Receive

        /// <summary>
        /// This method is called externally by protocol class when a connection is first received
        /// Basically it should be called for stablishing a network protocol
        /// </summary>
        public void onConnectionInit()
        {
            NetworkMessage msg = new NetworkMessage();
            Stream.BeginRead(msg.Buffer, 0, 2, new AsyncCallback(onReceiveFirstConnectionPacket), msg);
        }

        #endregion Connection Receive

        #region Callbacks

        private void onReceiveFirstConnectionPacket(IAsyncResult result)
        {
            if (!EndRead(result)) 
                return;

            NetworkMessage msg = (NetworkMessage)result.AsyncState;

            byte packetType = msg.ReadByte();
            switch (packetType)
            {
                case 0x01:
                    short OS = msg.ReadShort();
                    short Version = msg.ReadShort();
                    msg.ReadBytes(12); // Spr, Dat & Pic Signatures
                    if (Version < 760)
                    {
                        SendLoginDisconnectMessage("You need to use Tibia 7.6!");
                    }
                    else
                    {
                        int accountNumber = msg.ReadInteger();
                        string accountPassword = msg.ReadString();
                        SendCharacterList();
                    }
                    break;
            }

            Disconnect();
        }

        /// <summary>
        /// Ends the summary NetworkStream connection BeginRead method
        /// </summary>
        /// <param name="result"></param>
        /// <returns>false: connection died / disconnected</returns>
        private bool EndRead(IAsyncResult result)
        {
            NetworkMessage msg = (NetworkMessage)result.AsyncState;
            int value = Stream.EndRead(result);
            if (value == 0)
            {
                return false;
            }
            int size = (int)BitConverter.ToUInt16(msg.Buffer, 0) + 2;
            while (value < size) // Not sure if a while loop is affordable for this task
            {
                if (Stream.CanRead)
                    value += Stream.Read(msg.Buffer, value, size - value);
            }
            msg.Length = size;
            msg.Position = 0;
            msg.ReadShort(); // Size
            return true;
        }

        #endregion Callbacks

        #region Protocol Send

        private void SendLoginDisconnectMessage(string Message)
        {
            NetworkMessage msg = new NetworkMessage();
            msg.WriteByte(0x0A);
            msg.WriteString(Message);
            Send(msg);
        }

        private void SendCharacterList()
        {
            NetworkMessage msg = new NetworkMessage();
            msg.WriteByte(0x14);
            msg.WriteString("1\nWelcome to OTSharp!");
            msg.WriteByte(0x64);
            msg.WriteByte(0x03); // Character Amount
            msg.WriteString("Daniel");
            msg.WriteString("OTSharp");
            msg.WriteBytes(IPAddress.Parse("127.0.0.1").GetAddressBytes());
            msg.WriteShort(7172);
            msg.WriteString("Alejandro");
            msg.WriteString("OTSharp");
            msg.WriteBytes(IPAddress.Parse("127.0.0.1").GetAddressBytes());
            msg.WriteShort(7172);
            msg.WriteString("Mujica");
            msg.WriteString("OTSharp");
            msg.WriteBytes(IPAddress.Parse("127.0.0.1").GetAddressBytes());
            msg.WriteShort(7172);
            msg.WriteShort(0); // Premium Days
            Send(msg);
        }
        
        #endregion
    }
}
