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
using GameServer.Utils;
using GameServer.Environment;
using System.IO;

namespace GameServer.Network
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
            KnownCreatures = new HashSet<int>();
            ConnectionSocket = activeSocket;
            Stream = new NetworkStream(ConnectionSocket);
        }

        #endregion

        #region Properties

        private Socket ConnectionSocket { get; set; }
        private NetworkStream Stream { get; set; }

        public HashSet<int> KnownCreatures;
        public Player Player { get; set; }
        public bool LoggedIn { get; set; }

        #endregion

        #region Functioning

        /// <summary>
        /// Close this connection network
        /// </summary>
        public void Disconnect()
        {
            Stream.Close();
            ConnectionSocket.Close();
            LoggedIn = false;
        }

        /// <summary>
        /// Write a network packet to the active connection socket
        /// </summary>
        /// <param name="msg"></param>
        public void Send(NetworkMessage msg)
        {
            try
            {
                msg.AddPacketSize();
                Stream.Write(msg.Buffer, 0, msg.Length);
            }
            catch // Could sometimes throw a ObjectDisposedException
                  // Most likely when the client debugs or crashes
            {
                Disconnect();
            }
        }

        /// <summary>
        /// Check if a creature is known
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="IsKnown"></param>
        /// <param name="RemovedKnown"></param>
        private void CheckCreatureAsKnown(int Id, ref bool IsKnown, ref int RemovedKnown)
        {
            bool result = KnownCreatures.Add(Id);
            if (!result)
            {
                IsKnown = true;
                return;
            }

            IsKnown = false;

            if (KnownCreatures.Count > 150)
            {
                foreach (int knownId in KnownCreatures)
                {
                    Creature creature = Game.getCreature(knownId);
                    if (!Player.CanSee(creature))
                    {
                        RemovedKnown = knownId;
                        KnownCreatures.Remove(knownId);
                        return;
                    }
                }

                // Bad situation, let's just remove anyone.
                int firstKnown = KnownCreatures.First();
                if (firstKnown == Id)
                {
                    firstKnown++;
                }

                RemovedKnown = firstKnown;
                KnownCreatures.Remove(firstKnown);
            }
            else
            {
                RemovedKnown = 0;
            }
        }

        #endregion

        #region Connection Receive

        /// <summary>
        /// This method is called externally by protocol class when a connection is first received
        /// </summary>
        public void onConnectionInit()
        {
            NetworkMessage msg = new NetworkMessage();
            Stream.BeginRead(msg.Buffer, 0, NetworkMessage.MAX_BUFFER_SIZE, new AsyncCallback(onReceiveFirstConnectionPacket), msg);
        }

        /// <summary>
        /// Listens for incoming packets from the client
        /// </summary>
        private void listenConnection()
        {
            try
            {
                if (Player == null || !LoggedIn)
                {
                    Disconnect();
                    return;
                }

                NetworkMessage msg = new NetworkMessage();
                Stream.BeginRead(msg.Buffer, 0, NetworkMessage.MAX_BUFFER_SIZE, new AsyncCallback(onIncomingPacket), msg);
            }
            catch
            {
                Disconnect();
            }
        }

        private void onIncomingPacket(IAsyncResult result)
        {
            if (!EndRead(result))
            {
                Disconnect();
                return;
            }

            NetworkMessage msg = (NetworkMessage)result.AsyncState;
            byte packetType = msg.ReadByte();
            parsePacket(packetType, msg);
        }

        private void parsePacket(byte packetType, NetworkMessage msg)
        {
#if DEBUG
            Console.WriteLine("{0} sent 0x{1}.", Player.Name, packetType.ToString("X2"));
#endif
            switch (packetType)
            {
                case 0x14: // Logout
                    Disconnect();
                    Game.RemoveCreature(Player);
            	    break;
                case 0x1E: // Keep Alive / Ping Response
                    break;
                case 0x64: // Auto Walk
                    break;
                case 0x65: // Move North
                    Game.MoveCreature(Player, Direction.North);
                    break;
                case 0x66: // Move East
                    Game.MoveCreature(Player, Direction.East);
                    break;
                case 0x67: // Move South
                    Game.MoveCreature(Player, Direction.South);
                    break;
                case 0x68: // Move West
                    Game.MoveCreature(Player, Direction.West);
                    break;
                case 0x69: // Stop-AutoWalk
                    break;
                case 0x6A: // Move North East
                    Game.MoveCreature(Player, Direction.NorthEast);
                    break;
                case 0x6B: // Move South East
                    Game.MoveCreature(Player, Direction.SouthEast);
                    break;
                case 0x6C: // Move South West
                    Game.MoveCreature(Player, Direction.SouthWest);
                    break;
                case 0x6D: // Move North West
                    Game.MoveCreature(Player, Direction.NorthWest);
                    break;
                case 0x6F: // Turn North
                    break;
                case 0x70: // Turn East
                    break;
                case 0x71: // Turn South
                    break;
                case 0x72: // Turn West
                    break;
                case 0x78: // Move Item
                    break;
                case 0x7D: // Request Trade
                    break;
                case 0x7E: // Look at an item in trade
                    break;
                case 0x7F: // Accept Trade
                    break;
                case 0x80: // Close/Cancel Trade
                    break;
                case 0x82: // Use Item
                    break;
                case 0x83: // Use Item Ex
                    break;
                case 0x84: // Battle Window
                    break;
                case 0x85: // Rotate Item
                    break;
                case 0x87: // Close Container
                    break;
                case 0x88: // "up-arrow" container
                    break;
                case 0x89: // Text Window
                    break;
                case 0x8A: // House Window
                    break;
                case 0x8C: // Look
                    break;
                case 0x96: // Say something
                    break;
                case 0x97: // Request Channels
                    break;
                case 0x98: // Open Channel
                    break;
                case 0x99: // Close Channel
                    break;
                case 0x9A: // Open Private Channel
                    break;
                case 0x9B: // Process Rule Violation Report
                    break;
                case 0x9C: // GM closes Rule Violation Report
                    break;
                case 0x9D: // Player Cancels Report
                    break;
                case 0xA0: // Fight Modes
                    break;
                case 0xA1: // Attack
                    break;
                case 0xA2: // Follow
                    break;
                case 0xA3: // Invite Party
                    break;
                case 0xA4: // Join Party
                    break;
                case 0xA5: // Remove Party Invitation
                    break;
                case 0xA6: // Pass Party Leadership
                    break;
                case 0xA7: // Leave Party
                    break;
                case 0xAA: // Create Private Channel
                    break;
                case 0xAB: // Channel Invite
                    break;
                case 0xAC: // Channel Exclude
                    break;
                case 0xBE: // Cancel Move
                    break;
                case 0xC9: // Client Requests to resend tile
                    break;
                case 0xCA: // Client Requests to resend a container (happens when you store more than a container maxsize)
                    break;
                case 0xD2: // Requets Outfits
                    break;
                case 0xD3: // Set Outfit
                    break;
                case 0xDC: // Add VIP
                    break;
                case 0xDD: // Remove VIP
                    break;
                case 0xE6: // Bug Report
                    break;
                case 0xE7: // Violation Window
                    break;
                case 0xE8: // Debug Assertion
                    break;
                default:
                    Console.WriteLine("{0} sent packet 0x{1} which is unknown.", Player.Name, packetType.ToString("X2"));
                    break;
            }

            listenConnection();
        }

        #endregion Connection Receive

        #region Callbacks

        private void onReceiveFirstConnectionPacket(IAsyncResult result)
        {
            if (!EndRead(result))
            {
                Disconnect();
                return;
            }

            NetworkMessage msg = (NetworkMessage)result.AsyncState;

            byte packetType = msg.ReadByte();
            switch (packetType)
            {
                case 0x0A:
                    short OS = msg.ReadShort();
                    short Version = msg.ReadShort();
                    if (Version != 760)
                    {
                        SendLoginDisconnectMessage("Only Tibia clients of version 7.60!");
                        Disconnect();
                    }
                    else
                    {
                        bool GMClient = msg.ReadByte() == 1;
                        int accountNumber = msg.ReadInteger();
                        string character = msg.ReadString();
                        string accountPassword = msg.ReadString();

                        processLogin(accountNumber, accountPassword, character);
                    }
                    break;
                default:
                    Disconnect();
                    break;
            }
        }

        /// <summary>
        /// Ends the summary NetworkStream connection BeginRead method
        /// </summary>
        /// <param name="result"></param>
        /// <returns>false: connection died / disconnected</returns>
        private bool EndRead(IAsyncResult result)
        {
            try
            {
                NetworkMessage msg = (NetworkMessage)result.AsyncState;
                int value = Stream.EndRead(result);
                if (value == 0)
                {
                    Disconnect();
                    return false;
                }

                int size = (int)BitConverter.ToUInt16(msg.Buffer, 0) + 2;
                msg.Length = size;
                msg.Position = 0;
                msg.ReadShort(); // Size
            }
            catch
            {
                Disconnect();
                return false;
            }

            return true;
        }

        #endregion Callbacks

        #region Login Processing

        private void processLogin(int accountNumber, string password, string character)
        {
            Player = new Player();
            Player.Name = character;
            Player.Connection = this;

            if (Game.canPlayerSpawnAt(Player, new Position(50, 50, 7)))
            {
                Game.setPlayerOnMap(Player, new Position(50, 50, 7));
                LoggedIn = true;
                SendJoinGame();

                listenConnection();
            }
            else
            {
                SendLoginDisconnectMessage("Invalid position.");
                Disconnect();
            }
        }

        #endregion

        #region Protocol Add

        private void AddTileCreature(NetworkMessage msg, Creature creature, byte stackPos)
        {
            if (stackPos < 10)
            {
                msg.WriteByte(0x6A);
                AddPosition(msg, creature.Position);
                bool known = false;
                int removed = 0;
                CheckCreatureAsKnown(creature.Id, ref known, ref removed);
                AddCreature(msg, creature, known, removed);
            }
        }

        private void AddMapDescription(NetworkMessage msg, Position pos)
        {
            msg.WriteByte(0x64);
            AddPosition(msg, pos);
            GetMapDescription(pos.X - 8, pos.Y - 6, pos.Z, 18, 14, msg);
        }

        private void GetMapDescription(int X, int Y, int Z, int Width, int Height, NetworkMessage msg)
        {
            int skip = -1;
            int startz, endz, zstep;

            if (Z > 7)
            {
                startz = Z - 2;
                endz = Math.Min(15, Z + 2);
                zstep = 1;
            }
            else
            {
                startz = 7;
                endz = 0;
                zstep = -1;
            }

            for (int nz = startz; nz != endz + zstep; nz += zstep)
            {
                GetFloorDescription(msg, X, Y, nz, Width, Height, Z - nz, ref skip);
            }

            if (skip >= 0)
            {
                msg.WriteByte((byte)skip);
                msg.WriteByte(0xFF);
            }
        }

        private void GetFloorDescription(NetworkMessage msg, int X, int Y, int Z, int Width, int Height, int Offset, ref int Skip)
        {
            for (int nx = 0; nx < Width; nx++)
            {
                for (int ny = 0; ny < Height; ny++)
                {
                    Tile tile = Game.Map.getTile(new Position(X + nx + Offset, Y + ny + Offset, (byte)Z));
                    if (tile != null)
                    {
                        if (Skip >= 0)
                        {
                            msg.WriteByte((byte)Skip);
                            msg.WriteByte(0xFF);
                        }
                        Skip = 0;
                        GetTileDescription(tile, msg);
                    }
                    else if (Skip == 0xFE)
                    {
                        msg.WriteByte(0xFF);
                        msg.WriteByte(0xFF);
                        Skip = -1;
                    }
                    else
                    {
                        ++Skip;
                    }
                }
            }
        }

        private void GetTileDescription(Tile tile, NetworkMessage msg)
        {
            if (tile != null)
            {
                int count = 0;
                if (tile.GroundItem != null)
                {
                    AddItem(msg, tile.GroundItem);
                    count++;
                }

                //TODO: Top Items

                foreach (Creature c in tile.Creatures)
                {
                    if (Player.CanSee(c))
                    {
                        bool known = false;
                        int removed = 0;
                        CheckCreatureAsKnown(c.Id, ref known, ref removed);
                        AddCreature(msg, c, known, removed);
                        if (count++ >= 10)
                            return;
                    }
                }

                //TODO: Down Items
            }
        }

        private void AddCreature(NetworkMessage msg, Creature creature, bool IsKnown, int RemoveKnown)
        {
            if (IsKnown)
            {
                msg.WriteShort(0x62);
                msg.WriteInt(creature.Id);
            }
            else
            {
                msg.WriteShort(0x61);
                msg.WriteInt(RemoveKnown);
                msg.WriteInt(creature.Id);
                msg.WriteString(creature.Name);
            }

            msg.WriteByte(creature.getHealthPercentage());
            msg.WriteByte((byte)creature.Direction);
            AddOutfit(msg, creature.Outfit);
            msg.WriteByte(creature.Light.Radius);
            msg.WriteByte(creature.Light.Color);
            msg.WriteShort((short)creature.Speed);
            msg.WriteByte((byte)creature.Skull);
            msg.WriteByte((byte)creature.Shield);
        }

        private void AddItem(NetworkMessage msg, Item item)
        {
            msg.WriteShort(item.Id);
        }

        private void AddCreatureLight(NetworkMessage msg, Creature creature)
        {
            msg.WriteByte(0x8D);
            msg.WriteInt(creature.Id);
            msg.WriteByte(creature.Light.Radius);
            msg.WriteByte(creature.Light.Color);
        }

        private void AddPosition(NetworkMessage msg, Position pos)
        {
            msg.WriteShort((short)pos.X);
            msg.WriteShort((short)pos.Y);
            msg.WriteByte((byte)pos.Z);
        }

        private void AddOutfit(NetworkMessage msg, Outfit outfit)
        {
            msg.WriteByte(outfit.Type);
            msg.WriteByte(outfit.Head);
            msg.WriteByte(outfit.Body);
            msg.WriteByte(outfit.Legs);
            msg.WriteByte(outfit.Feet);
        }

        private void AddPlayerStats(NetworkMessage msg)
        {
            msg.WriteByte(0xA0);
            msg.WriteShort((short)Player.Health);
            msg.WriteShort((short)Player.MaxHealth);
            msg.WriteShort((short)Player.Capacity);
            msg.WriteInt(Player.Experience);
            msg.WriteShort((short)Player.Level);
            msg.WriteByte(Player.getLevelPercentage());
            msg.WriteShort((short)Player.Mana);
            msg.WriteShort((short)Player.MaxMana);
            msg.WriteByte(Player.MagicLevel);
            msg.WriteByte(Player.getMagicLevelPercentage());
            msg.WriteByte(Player.Soul);
        }

        private void AddPlayerSkills(NetworkMessage msg)
        {
            msg.WriteByte(0xA1);
            msg.WriteByte(Player.FishingLevel);
            msg.WriteByte(0);
            msg.WriteByte(Player.ClubFightingLevel);
            msg.WriteByte(0);
            msg.WriteByte(Player.SwordFightingLevel);
            msg.WriteByte(0);
            msg.WriteByte(Player.AxeFightingLevel);
            msg.WriteByte(0);
            msg.WriteByte(Player.DistanceFightingLevel);
            msg.WriteByte(0);
            msg.WriteByte(Player.ShieldingLevel);
            msg.WriteByte(0);
            msg.WriteByte(Player.FishingLevel);
            msg.WriteByte(0);
        }

        private void AddWorldLight(NetworkMessage msg)
        {
            msg.WriteByte(0x82);
            msg.WriteByte(Game.getWorldAmbiente().Radius);
            msg.WriteByte(Game.getWorldAmbiente().Color);
        }

        private void AddInventoryItem(NetworkMessage msg, Item item, Slot slot)
        {
            if (item == null)
            {
                msg.WriteByte(0x79);
                msg.WriteByte((byte)slot);
            }
            else
            {
                msg.WriteByte(0x78);
                msg.WriteByte((byte)slot);
                AddItem(msg, item);
            }
        }

        private void AddRemoveTileItem(NetworkMessage msg, Position pos, byte stack)
        {
            if (stack < 10)
            {
                msg.WriteByte(0x6C);
                AddPosition(msg, pos);
                msg.WriteByte(stack);
            }
        }

        private void AddMoveDownCreature(NetworkMessage msg, Creature creature, Tile toTile, Tile fromTile, byte oldStack)
        {
            if (creature == Player)
            {
                msg.WriteByte(0xBF);
                // From surface to underground
                if (toTile.Position.Z == 8)
                {
                    int skip = -1;
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, toTile.Position.Z, 18, 14, -1, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, toTile.Position.Z + 1, 18, 14, -2, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, toTile.Position.Z + 2, 18, 14, -3, ref skip);

                    if (skip >= 0)
                    {
                        msg.WriteByte((byte)skip);
                        msg.WriteByte(0xFF);
                    }
                }
                // Going further down underground
                else if (toTile.Position.Z > fromTile.Position.Z && toTile.Position.Z > 8 && toTile.Position.Z < 14)
                {
                    int skip = -1;
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, toTile.Position.Z + 2, 18, 14, -3, ref skip);

                    if (skip >= 0)
                    {
                        msg.WriteByte((byte)skip);
                        msg.WriteByte(0xFF);
                    }
                }

                // Moving down makes us desynchronizes
                // For East
                msg.WriteByte(0x66);
                GetMapDescription(fromTile.Position.X + 9, fromTile.Position.Y - 7, toTile.Position.Z, 1, 14, msg);
                // South
                msg.WriteByte(0x67);
                GetMapDescription(fromTile.Position.X - 8, fromTile.Position.Y + 7, toTile.Position.Z, 18, 1, msg);
            }
        }

        private void AddMoveUpCreature(NetworkMessage msg, Creature creature, Tile toTile, Tile fromTile, byte oldStack)
        {
            if (creature == Player)
            {
                msg.WriteByte(0xBE);

                // Going to surface
                if (toTile.Position.Z == 7)
                {
                    int skip = -1;
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, 5, 18, 14, 3, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, 4, 18, 14, 4, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, 3, 18, 14, 5, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, 2, 18, 14, 6, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, 1, 18, 14, 7, ref skip);
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, 0, 18, 14, 8, ref skip);

                    if (skip >= 0)
                    {
                        msg.WriteByte((byte)skip);
                        msg.WriteByte(0xFF);
                    }
                }
                // Underground going upper grounds
                else if (toTile.Position.Z > fromTile.Position.Z && toTile.Position.Z > 8 && toTile.Position.Z < 14)
                {
                    int skip = -1;
                    GetFloorDescription(msg, fromTile.Position.X - 8, fromTile.Position.Y - 6, fromTile.Position.Z - 3, 18, 14, 3, ref skip);

                    if (skip >= 0)
                    {
                        msg.WriteByte((byte)skip);
                        msg.WriteByte(0xFF);
                    }
                }

                // Moving up floor desynchronizes
                // For west
                msg.WriteByte(0x68);
                GetMapDescription(fromTile.Position.X - 8, fromTile.Position.Y + 1 - 6, toTile.Position.Z, 1, 14, msg);
                // North
                msg.WriteByte(0x65);
                GetMapDescription(fromTile.Position.X - 8, fromTile.Position.Y - 6, toTile.Position.Z, 18, 1, msg);
            }
        }

        private void AddMagicEffect(NetworkMessage msg, Position position, byte effect)
        {
            msg.WriteByte(0x83);
            AddPosition(msg, position);
            msg.WriteByte(effect);
        }

        #endregion

        #region Protocol Send

        public void SendLoginDisconnectMessage(string Message)
        {
            NetworkMessage msg = new NetworkMessage();
            msg.WriteByte(0x14);
            msg.WriteString(Message);
            Send(msg);
        }

        public void SendJoinGame()
        {
            NetworkMessage msg = new NetworkMessage();
            msg.WriteByte(0x0A);
            msg.WriteInt(Player.Id);
            msg.WriteShort(0x0032 /* Client-Side Drawing Speed */);
            msg.WriteByte(0x00 /* Can Report Bugs */);

            AddMapDescription(msg, Player.Position);

            AddInventoryItem(msg, Player.Inventory[(int)Slot.Head], Slot.Head);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Amulet], Slot.Amulet);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Backpack], Slot.Backpack);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Armor], Slot.Armor);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Right], Slot.Right);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Left], Slot.Left);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Legs], Slot.Legs);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Feet], Slot.Feet);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Right], Slot.Ring);
            AddInventoryItem(msg, Player.Inventory[(int)Slot.Extra], Slot.Extra);

            AddPlayerStats(msg);
            AddPlayerSkills(msg);

            AddMagicEffect(msg, Player.Position, 4);
            AddWorldLight(msg);

            Send(msg);
        }

        public void SendAddCreature(Creature creature, byte stackPos)
        {
            NetworkMessage msg = new NetworkMessage();
            AddTileCreature(msg, creature, stackPos);
            Send(msg);
        }

        public void SendRemoveCreature(Creature creature, byte stackPos)
        {
            NetworkMessage msg = new NetworkMessage();
            AddRemoveTileItem(msg, creature.Position, stackPos);
            Send(msg);
        }

        public void SendCancelWalk()
        {
            NetworkMessage msg = new NetworkMessage();
            msg.WriteByte(0xB5);
            msg.WriteByte((byte)Player.Direction);
            Send(msg);
        }

        public void SendCreatureMove(Creature creature, Tile newTile, byte newStackPos, Tile oldTile, byte oldStackPos, bool Teleport = false)
        {
            NetworkMessage msg = new NetworkMessage();
            if (creature == Player)
            {
                if (Teleport || oldStackPos >= 10)
                {
                    AddRemoveTileItem(msg, oldTile.Position, oldStackPos);
                    AddMapDescription(msg, newTile.Position);
                }
                else
                {
                    if (oldTile.Position.Z == 7 && newTile.Position.Z >= 8)
                    {
                        AddRemoveTileItem(msg, oldTile.Position, oldStackPos);
                    }
                    else
                    {
                        msg.WriteByte(0x6D);
                        AddPosition(msg, oldTile.Position);
                        msg.WriteByte(oldStackPos);
                        AddPosition(msg, newTile.Position);
                    }

                    // Floor Changing Down
                    if (newTile.Position.Z > oldTile.Position.Z)
                    {
                        AddMoveDownCreature(msg, creature, newTile, oldTile, oldStackPos);
                    }
                    // Floor Changing Up
                    else if (newTile.Position.Z < oldTile.Position.Z)
                    {
                        AddMoveUpCreature(msg, creature, newTile, oldTile, oldStackPos);
                    }

                    if (oldTile.Position.Y > newTile.Position.Y) // North, for old x
                    {
                        msg.WriteByte(0x65);
                        GetMapDescription(oldTile.Position.X - 8, newTile.Position.Y - 6, newTile.Position.Z, 18, 1, msg);
                    }
                    else if (oldTile.Position.Y < newTile.Position.Y) // South, for old x
                    {
                        msg.WriteByte(0x67);
                        GetMapDescription(oldTile.Position.X - 8, newTile.Position.Y + 7, newTile.Position.Z, 18, 1, msg);
                    }

                    if (oldTile.Position.X < newTile.Position.X) // East, with new Y
                    {
                        msg.WriteByte(0x66);
                        GetMapDescription(newTile.Position.X + 9, newTile.Position.Y - 6, newTile.Position.Z, 1, 14, msg);
                    }
                    else if (oldTile.Position.X > newTile.Position.X) // West, with new Y
                    {
                        msg.WriteByte(0x68);
                        GetMapDescription(newTile.Position.X - 8, newTile.Position.Y - 6, newTile.Position.Z, 1, 14, msg);
                    }
                }
                Send(msg);
            }
            else if (Player.CanSee(oldTile.Position) && Player.CanSee(newTile.Position))
            {
                if (Player.CanSee(creature))
                {
                    if (Teleport || (oldTile.Position.Z == 7 && newTile.Position.Z >= 8) || oldStackPos >= 10)
                    {
                        AddRemoveTileItem(msg, oldTile.Position, oldStackPos);
                        AddTileCreature(msg, creature, oldStackPos);
                    }
                    else
                    {
                        msg.WriteByte(0x6D);
                        AddPosition(msg, oldTile.Position);
                        msg.WriteByte(oldStackPos);
                        AddPosition(msg, newTile.Position);
                    }
                    Send(msg);
                }
            }
            else if (Player.CanSee(oldTile.Position))
            {
                if (Player.CanSee(creature))
                {
                    AddRemoveTileItem(msg, oldTile.Position, oldStackPos);
                    Send(msg);
                }
            }
            else if (Player.CanSee(newTile.Position))
            {
                if (Player.CanSee(creature))
                {
                    AddTileCreature(msg, creature, newStackPos);
                    Send(msg);
                }
            }
        }

        #endregion
    }
}
