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

namespace GameServer.Utils
{
    /// <summary>
    /// Creature Look type
    /// </summary>
    public struct Outfit
    {
        public Outfit(byte tType = 128, byte tHead = 0, byte tBody = 0, byte tLegs = 0, byte tFeet = 0, short tEx = 0)
        {
            Type = tType;
            Head = tHead;
            Body = tBody;
            Legs = tLegs;
            Feet = tFeet;
            Ex = tEx;
        }

        public byte Type;
        public byte Head;
        public byte Body;
        public byte Legs;
        public byte Feet;

        public short Ex;
    }
}
