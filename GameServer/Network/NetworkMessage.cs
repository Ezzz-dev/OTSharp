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

namespace GameServer.Network
{
    /// <summary>
    /// Manage a networking buffer to use between TCP clients
    /// </summary>
    public class NetworkMessage
    {
        public const int MAX_BUFFER_SIZE = 0xFFFF;
        private int p_length;
        private int p_position;
        private byte[] p_buffer;

        public NetworkMessage()
        {
            p_buffer = new byte[MAX_BUFFER_SIZE];
            p_length = 0;
            p_position = 2;
        }

        /// <summary>
        /// Get the current buffer byte array
        /// </summary>
        public byte[] Buffer { get { return p_buffer; } }

        /// <summary>
        /// Get/Set the current length of the buffer byte array
        /// </summary>
        public int Length { get { return p_length; } set { p_length = value; } }

        /// <summary>
        /// Get/Set the current position of the buffer byte array
        /// </summary>
        public int Position { get { return p_position; } set { p_position = value; } }

        #region Prepare

        /// <summary>
        /// Add packet size to the buffer array
        /// Used when sending the packet to an active connection
        /// </summary>
        public void AddPacketSize()
        {
            Array.Copy(BitConverter.GetBytes((short)p_length - 2), 0, p_buffer, 0, 2);
        }

        /// <summary>
        /// Sets the current buffer position after the packet size in order to read the real packet data
        /// </summary>
        public void PrepareToRead()
        {
            p_position = 0;
            byte[] buffer = new byte[2];
            Array.Copy(p_buffer, p_position, buffer, 0, 2);
            p_length = BitConverter.ToInt16(buffer, 0) + 2;
            p_position = 2;
        }

        #endregion Prepare

        #region Writing Functions

        /// <summary>
        /// Write a set of bytes to the buffer array
        /// </summary>
        /// <param name="values"></param>
        public void WriteBytes(byte[] values)
        {
            Array.Copy(values, 0, p_buffer, p_position, values.Length);
            p_position += values.Length;
            p_length = p_position;
        }

        /// <summary>
        /// Write a single byte to the buffer array
        /// </summary>
        /// <param name="value"></param>
        public void WriteByte(byte value)
        {
            p_buffer[p_position++] = value;
            p_length = p_position;
        }

        /// <summary>
        /// Write an unsigned short number to the buffer array
        /// </summary>
        /// <param name="value"></param>
        public void WriteShort(short value)
        {
            WriteBytes(BitConverter.GetBytes(Convert.ToUInt16(value)));
        }

        /// <summary>
        /// Write an unsigned integer to the buffer array
        /// </summary>
        /// <param name="value"></param>
        public void WriteInt(int value)
        {
            WriteBytes(BitConverter.GetBytes(Convert.ToUInt32(value)));
        }

        /// <summary>
        /// Write a string to the buffer array
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(string value)
        {
            WriteShort((short)value.Length);
            WriteBytes(ASCIIEncoding.ASCII.GetBytes(value));
        }

        #endregion Writing Functions

        #region Reading Functions

        /// <summary>
        /// Read a set of bytes from the buffer array
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int count)
        {
            if (p_position + count > p_length)
                return new byte[MAX_BUFFER_SIZE];
            byte[] buffer = new byte[count];
            Array.Copy(p_buffer, p_position, buffer, 0, count);
            p_position += count;
            return buffer;
        }

        /// <summary>
        /// Read a single byte from the buffer
        /// </summary>
        /// <returns></returns>
        public byte ReadByte()
        {
            if (p_position + 1 > p_length)
                return 0x00;
            return p_buffer[p_position++];
        }

        /// <summary>
        /// Read an unsigned short number from the buffer
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            if (p_position + 2 > p_length)
                return 0;
            return BitConverter.ToInt16(ReadBytes(2), 0);
        }

        /// <summary>
        /// Read an unsigned integer from the buffer
        /// </summary>
        /// <returns></returns>
        public int ReadInteger()
        {
            if (p_position + 4 > p_length)
                return 0;
            return BitConverter.ToInt32(ReadBytes(4), 0);
        }

        /// <summary>
        /// Read a string from the buffer
        /// </summary>
        /// <returns></returns>
        public string ReadString()
        {
            short length = ReadShort();
            byte[] data = ReadBytes(length);
            return ASCIIEncoding.Default.GetString(data);
        }

        #endregion Reading Functions
    }
}
