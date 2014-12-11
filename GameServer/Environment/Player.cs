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
using GameServer.Network;

namespace GameServer.Environment
{
    /// <summary>
    /// Handle a Game Player
    /// </summary>
    public class Player : Creature
    {
        #region Initializator

        /// <summary>
        /// Initialize
        /// </summary>
        public Player() : base()
        {
            Level = 1;
            Experience = 0;
            Capacity = 400;
            Health = 150;
            MaxHealth = 150;
            Mana = 0;
            MaxMana = 0;
            Soul = 100;

            MagicLevel = 0;
            FistFightingLevel = 10;
            SwordFightingLevel = 10;
            AxeFightingLevel = 10;
            ClubFightingLevel = 10;
            ShieldingLevel = 10;
            DistanceFightingLevel = 10;
            FishingLevel = 10;

            Outfit = new Outfit(128, 10, 20, 30, 40);
            Inventory = new Item[(int)Slot.Last];

            Connection = null;
            PrivateChatChannel = null;
        }

        #endregion

        #region Properties

        public byte Level { get; set; }
        public int Experience { get; set; }
        public int Capacity { get; set; }
        public short Mana { get; set; }
        public short MaxMana { get; set; }
        public byte Soul { get; set; }

        public byte MagicLevel { get; set; }
        public int ManaSpent { get; set; }

        public byte FistFightingLevel { get; set; }
        public byte FistFightingTries { get; set; }
        public byte SwordFightingLevel { get; set; }
        public byte SwordFightingTries { get; set; }
        public byte ClubFightingLevel { get; set; }
        public byte ClubFightingTries { get; set; }
        public byte AxeFightingLevel { get; set; }
        public byte AxeFightingTries { get; set; }
        public byte ShieldingLevel { get; set; }
        public byte ShieldingTries { get; set; }
        public byte DistanceFightingLevel { get; set; }
        public byte DistanceFightingTries { get; set; }
        public byte FishingLevel { get; set; }
        public byte FishingTries { get; set; }

        public Item[] Inventory;

        public Connection Connection { get; set; }
        public Channel PrivateChatChannel { get; set; }

        #endregion

        #region Functions

        /// <summary>
        /// Get current percentage to level up
        /// </summary>
        /// <returns></returns>
        public byte getLevelPercentage()
        {
            return 0;
        }

        /// <summary>
        /// Get current percentage to level up magic
        /// </summary>
        /// <returns></returns>
        public byte getMagicLevelPercentage()
        {
            return 0;
        }

        #endregion

        #region Events

        /// <summary>
        /// Event called when the player dies
        /// </summary>
        /// <returns></returns>
        public override bool onDie()
        {
            return base.onDie();
        }

        #endregion
    }
}
