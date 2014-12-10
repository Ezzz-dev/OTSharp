using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using GameServer.Environment;
using GameServer.Utils;

namespace GameServer.DataWorkers
{
    public class MapReader
    {
        private string fileName;
        private string lastError;
        byte[] buffer = new byte[128];

        public MapReader(string fileName)
        {
            this.fileName = fileName;
        }

        public bool GetMapTiles(Map map)
        {
            FileManager loader = new FileManager();
            loader.OpenFile(fileName);
            Node node = loader.GetRootNode();

            PropertyReader props;

            if (!loader.GetProps(node, out props))
            {
                lastError = "Could not read root property.";
                return false;
            }

            uint version = props.ReadUInt32();
            ushort width = props.ReadUInt16();
            ushort height = props.ReadUInt16();
            uint majorVersionItems = props.ReadUInt32();
            uint minorVersionItems = props.ReadUInt32();

            node = node.Child;

            if ((MapNodeType)node.Type != MapNodeType.MapData)
            {
                lastError = "Could not read data node.";
                return false;
            }

            if (!loader.GetProps(node, out props))
            {
                lastError = "Could not read map data attributes.";
                return false;
            }

            byte attribute;
            while (props.PeekChar() != -1)
            {
                attribute = props.ReadByte();
                switch ((MapAttribute)attribute)
                {
                    case MapAttribute.Description:
                       //TODO: map.Description = props.GetString();
                        break;
                    case MapAttribute.ExtSpawnFile:
                        //TODO: map.SpawnFile = props.GetString();
                        break;
                    case MapAttribute.ExtHouseFile:
                        //TODO: map.HouseFile = props.GetString();
                        break;
                    default:
                        lastError = "Unknown header node.";
                        return false;
                }
            }

            Node nodeMapData = node.Child;

            while (nodeMapData != null)
            {
                switch ((MapNodeType)nodeMapData.Type)
                {
                    case MapNodeType.TileArea:
                        if (!ParseTileArea(map, loader, nodeMapData)) return false;
                        break;
                    case MapNodeType.Towns:
                        if (!ParseTowns(map, loader, nodeMapData)) return false;
                        break;
                }
                nodeMapData = nodeMapData.Next;
            }

            return true;
        }

        /// <summary>
        /// Parse Tile-area.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="loader"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool ParseTileArea(Map map, FileManager loader, Node node)
        {
            PropertyReader props;
            if (!loader.GetProps(node, out props))
            {
                lastError = "Invalid map node.";
                return false;
            }

            int baseX = props.ReadUInt16();
            int baseY = props.ReadUInt16();
            int baseZ = props.ReadByte();

            Node nodeTile = node.Child;

            while (nodeTile != null)
            {
                if (nodeTile.Type == (long)MapNodeType.Tile ||
                    nodeTile.Type == (long)MapNodeType.HouseTile)
                {
                    loader.GetProps(nodeTile, out props);

                    int tileX = baseX + props.ReadByte();
                    int tileY = baseY + props.ReadByte();
                    int tileZ = baseZ;

                    Tile tile = new Tile();
                    tile.Position = new Position(tileX, tileY, tileZ);
                    // TODO: houses
                    if (nodeTile.Type == (long)MapNodeType.HouseTile)
                    {
                        uint houseId = props.ReadUInt32();
                    }

                    byte attribute;
                    while (props.PeekChar() != -1)
                    {
                        attribute = props.ReadByte();
                        switch ((MapAttribute)attribute)
                        {
                            case MapAttribute.TileFlags:
                                {
                                    TileFlags flags = (TileFlags)props.ReadUInt32();
                                    if ((flags & TileFlags.ProtectionZone) == TileFlags.ProtectionZone)
                                    {
                                        //TODO
                                    }
                                    else if ((flags & TileFlags.NoPvpZone) == TileFlags.NoPvpZone)
                                    {
                                        //TODO
                                    }
                                    else if ((flags & TileFlags.PvpZone) == TileFlags.PvpZone)
                                    {
                                        //TODO
                                    }

                                    if ((flags & TileFlags.NoLogout) == TileFlags.NoLogout)
                                    {
                                        //TODO
                                    }

                                    if ((flags & TileFlags.Refresh) == TileFlags.Refresh)
                                    {
                                        //TODO
                                    }
                                    break;
                                }
                            case MapAttribute.Item:
                                {
                                    ushort itemId = props.ReadUInt16();
                                    //TODO
                                    break;
                                }
                            default:
                                //TODO
                                return false;
                        }
                    }

                    Node nodeItem = nodeTile.Child;

                    while (nodeItem != null)
                    {
                        if (nodeItem.Type == (long)MapNodeType.Item)
                        {
                            loader.GetProps(nodeItem, out props);
                            short itemId = (short)props.ReadUInt16();
                            //TODO: Checks for Ground items.
                            Item item = new Item(itemId);
                            tile.GroundItem = item;
                        }
                        else
                        {
                            //TODO
                            return false;
                        }
                        nodeItem = nodeItem.Next;
                    }

                   //Set tile on Map.
                   map.setTile(tile.Position, tile);
                }
                nodeTile = nodeTile.Next;
            }

            return true;
        }

        /// <summary>
        /// Parse towns on OTBM map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="loader"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool ParseTowns(Map map, FileManager loader, Node node)
        {
            PropertyReader props;
            Node nodeTown = node.Child;
            while (nodeTown != null)
            {
                if (!loader.GetProps(nodeTown, out props))
                {
                    lastError = "Could not read town data.";
                    return false;
                }

                uint townid = props.ReadUInt32();
                string townName = props.GetString();
                ushort townTempleX = props.ReadUInt16();
                ushort townTempleY = props.ReadUInt16();
                byte townTempleZ = props.ReadByte();
                //TODO: Add town here.

                nodeTown = nodeTown.Next;
            }
            return true;
        }
    }
}
