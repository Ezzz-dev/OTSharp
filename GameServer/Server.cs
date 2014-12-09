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
using GameServer.Protocol;
using GameServer.Environment;

namespace GameServer
{
    class Server
    {
        static void Main(string[] args)
        {
            ProtocolGame protocol = new ProtocolGame("127.0.0.1", 7172);

            Console.WriteLine(":::::::::::::::::::::::::::::::::");
            Console.WriteLine(":: OTSharp Game Server");
            Console.WriteLine(":: Tibia 7.6");
            Console.WriteLine(":: Copyright © Daniel Alejandro");
            Console.WriteLine(":::::::::::::::::::::::::::::::::");

            Console.WriteLine(">> Initializing protocol...");
            if (!protocol.Start())
            {
                Console.WriteLine("Error: Login Server not running!");
            }
            else
            {
                if (!Game.Map.LoadMap())
                {
                    Console.WriteLine("Error: could not load map!");
                } 
                else
                {
                    Console.WriteLine(">> Game Server running!");
                }
            }

            Console.ReadKey(false);
        }
    }
}
