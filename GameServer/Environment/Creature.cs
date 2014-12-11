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
using GameServer.Utils;

namespace GameServer.Environment
{
    /// <summary>
    /// Client Creature
    /// </summary>
    public abstract class Creature
    {
        #region Initializator

        public Creature()
        {
            Direction = Direction.South;
            Skull = Skull.None;
        }

        #endregion

        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }
        public short Health { get; set; }
        public short MaxHealth { get; set; }
        public short Speed { get; set; }
        public Position Position { get; set; }
        public Outfit Outfit { get; set; }
        public Direction Direction { get; set; }
        public Light Light { get; set; }
        public Skull Skull { get; set; }
        public Shield Shield { get; set; }

        private Tile p_standingtile = null;
        public Tile StandingTile { get { return p_standingtile; } set { p_standingtile = value; Position = value.Position; } }

        #endregion

        #region Bool
        
        /// <summary>
        /// Check if can see a certain creature
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public bool CanSee(Creature creature)
        {
            return true;
        }

        /// <summary>
        /// Check if can see a certain position in the map
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>true: can see the position</returns>
        public bool CanSee(Position position)
        {
            if (Position.Z <= 7)
            {
                if (position.Z > 7)
                {
                    return false;
                }
            }
            else if (Position.Z >= 8)
            {
                if (Math.Abs(Position.Z - position.Z) > 2)
                {
                    return false;
                }
            }

            int offsetz = Position.Z - position.Z;
            if ((position.X >= Position.X - 8 + offsetz) && (position.X <= Position.X + 9 + offsetz) &&
                (position.Y >= Position.Y - 6 + offsetz) && (position.Y <= Position.Y + 7 + offsetz))
                return true;

            return false;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Get the HP percentage
        /// </summary>
        /// <returns></returns>
        public byte getHealthPercentage()
        {
            return 100;
        }

        #endregion

        #region Override Events

        /// <summary>
        /// Event called when a creature speaks
        /// </summary>
        /// <param name="creature"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public virtual bool onCreatureSpeak(Creature creature, TalkType type, string message)
        {
            return true;
        }

        /// <summary>
        /// Event called when this creature dies
        /// </summary>
        /// <returns></returns>
        public virtual bool onDie()
        {
            return true;
        }

        /// <summary>
        /// Event called when a creature appears
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public virtual bool onCreatureAppear(Creature creature)
        {
            return true;
        }

        /// <summary>
        /// Event called when a creature disappears
        /// </summary>
        /// <param name="creature"></param>
        /// <returns></returns>
        public virtual bool onCreatureDisappear(Creature creature)
        {
            return true;
        }

        #endregion
    }
}
