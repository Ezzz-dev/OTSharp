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
    public class Map
    {
        #region Properties

        private Dictionary<Position, Tile> Tiles;

        #endregion

        #region Initializator

        /// <summary>
        /// Initialize Map Class
        /// </summary>
        public Map()
        {
            Tiles = new Dictionary<Position, Tile>();
        }

        #endregion

        #region Load

        /// <summary>
        /// Load Map Area
        /// </summary>
        public bool LoadMap()
        {
            for (int x = 0; x != 100; x++)
            {
                for (int y = 0; y != 100; y++)
                {
                    Tile tile = new Tile();
                    tile.GroundItem = new Item(106);
                    tile.Position = new Position(x, y, 7);
                    Tiles.Add(tile.Position, tile);
                }
            }
            return true;
        }

        #endregion

        #region Get

        /// <summary>
        /// Get a tile from the map
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>null: tile doesn't exist</returns>
        public Tile getTile(Position pos)
        {
            Tile tile;
            Tiles.TryGetValue(pos, out tile);
            return tile;
        }

        #endregion
    }
}
