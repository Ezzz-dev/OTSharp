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
    public enum Skull : byte
    {
        None = 0,
        White = 1,
        Yellow = 2,
        Red = 3
    }

    public enum Shield : byte
    {
        None = 0,
    }

    public enum Direction : int
    {
        North = 0,
        East = 1,
        South = 2,
        West = 3,
        NorthEast = 4,
        NorthWest = 5,
        SouthEast = 6,
        SouthWest = 7
    }

    public enum Slot : byte
    {
        Head = 1,
        Amulet = 2,
        Backpack = 3,
        Armor = 4,
        Right = 5,
        Left = 6,
        Legs = 7,
        Feet = 8,
        Ring = 9,
        Extra = 10,
        Last = 11
    }

    public enum TalkType : byte
    {
        Say = 0x01,
        Whisper = 0x02,
        Yell = 0x03,
        PrivateChannel = 0x04,
        ChannelYellow = 0x05,
        RuleViolationChannel = 0x06,
        RuleViolationAnswer = 0x07,
        RuleViolationContinue = 0x08,
        Broadcast = 0x09,
        ChannelRed = 0x0A, // #c <text>
        PrivateChannelRed = 0x0B, // @player@ <text>
        ChannelOrange = 0x0C,
        ChannelRedAnonymous = 0x0D, // #d <text>
        MonsterSay = 0x10,
        MonsterYell = 0x11
    }

    public enum ChannelID : int
    {
        Guild = 0x00,
        RuleViolations = 0x03,
        GameChat = 0x04,
        Trade = 0x05,
        RLChat = 0x06,
        Help = 0x07, // Help channel must always be ID #7 (client side limitation)
        Staff = 0x08,
        Tutor = 0x09,
        Gamemaster = 0x10,
        Private = 0xFF
    }

    public enum MessageType : byte
    {
        ConsoleYellow = 0x01,
        ConsoleLightBlue = 0x04,
        ConsoleOrange = 0x11,
        ConsoleWarning = 0x12,
        EventAdvance = 0x13,
        EventDefault = 0x14,
        StatusDefault = 0x15,
        InfoDescription = 0x16,
        StatusSmall = 0x17,
        ConsoleBlue = 0x18,
        ConsoleRed = 0x19
    }
}
