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
    public struct Position
    {
        public Position(int tX, int tY, byte tZ)
        {
            X = tX;
            Y = tY;
            Z = tZ;
        }

        public int X;
        public int Y;
        public byte Z;

        /// <summary>
        /// Check if two positions are in the given range
        /// </summary>
        /// <param name="pos1">Default: 1</param>
        /// <param name="pos2">Default: 1</param>
        /// <returns></returns>
        public static bool AreInRange(Position pos1, Position pos2, int deltax = 1, int deltay = 1)
        {
            if (Math.Abs(pos1.X - pos2.X) > deltax ||
                Math.Abs(pos1.Y - pos2.Y) > deltay)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get an adjacent position based on the given direction
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Position getAdjacentPosition(Direction direction)
        {
            Position newPosition = new Position(this.X, this.Y, this.Z);
            if (direction == Direction.East)
            {
                newPosition.X++;
            }
            else if (direction == Direction.North)
            {
                newPosition.Y--;
            }
            else if (direction == Direction.South)
            {
                newPosition.Y++;
            }
            else if (direction == Direction.West)
            {
                newPosition.X--;
            }
            else if (direction == Direction.NorthEast)
            {
                newPosition.X++;
                newPosition.Y--;
            }
            else if (direction == Direction.NorthWest)
            {
                newPosition.X--;
                newPosition.Y--;
            }
            else if (direction == Direction.SouthEast)
            {
                newPosition.X++;
                newPosition.Y++;
            }
            else if (direction == Direction.SouthWest)
            {
                newPosition.X--;
                newPosition.Y++;
            }

            return newPosition;
        }
    }
}
