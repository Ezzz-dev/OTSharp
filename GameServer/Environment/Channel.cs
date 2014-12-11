using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Utils;

namespace GameServer.Environment
{
    /// <summary>
    /// Handle In-Game channels
    /// </summary>
    public static class Channels
    {
        #region Properties

        private static List<Channel> public_channels = new List<Channel>();
        private static List<Channel> special_channels = new List<Channel>();

        #endregion

        #region Functioning

        /// <summary>
        /// Initialize all channels adversity
        /// </summary>
        public static void Init()
        {
            // Public Channels
            Channel channel = new Channel(ChannelID.GameChat, "Game-Chat");
            public_channels.Add(channel);
            channel = new Channel(ChannelID.Help, "Help");
            public_channels.Add(channel);
            channel = new Channel(ChannelID.RLChat, "RL-Chat");
            public_channels.Add(channel);
            channel = new Channel(ChannelID.Trade, "Trade");
            public_channels.Add(channel);
            // Special Channels
            channel = new Channel(ChannelID.RuleViolations, "Rule-Violations");
            special_channels.Add(channel);
            channel = new Channel(ChannelID.Gamemaster, "Gamemaster");
            special_channels.Add(channel);
            channel = new Channel(ChannelID.Staff, "Staff");
            special_channels.Add(channel);
            channel = new Channel(ChannelID.Tutor, "Tutor");
            special_channels.Add(channel);
        }

        #endregion

        #region Get

        /// <summary>
        /// Get a list of channels for a given player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static List<Channel> getChannelsForPlayer(Player player)
        {
            List<Channel> toReturn = new List<Channel>();

            foreach (Channel channel in public_channels)
            {
                if (channel.canPlayerJoin(player))
                {
                    toReturn.Add(channel);
                }
            }

            foreach (Channel channel in special_channels)
            {
                if (channel.canPlayerJoin(player))
                {
                    toReturn.Add(channel);
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Get an existing channel with the given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Channel getChannelById(ChannelID id)
        {
            foreach (Channel channel in public_channels)
            {
                if (channel.Id == id)
                {
                    return channel;
                }
            }

            foreach (Channel channel in special_channels)
            {
                if (channel.Id == id)
                {
                    return channel;
                }
            }

            return null;
        }

        #endregion

        #region Functioning

        /// <summary>
        /// Player talks to the given channel
        /// </summary>
        /// <param name="player"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool PlayerTalkToChannel(Player player, ChannelID id, string message, TalkType type)
        {
            Channel channel = Channels.getChannelById(id);
            if (channel != null)
            {
                if (type != TalkType.RuleViolationAnswer)
                {
                    type = TalkType.ChannelYellow;
                }

                channel.PlayerTalk(player, type, message);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove a player from the given channel
        /// </summary>
        /// <param name="player"></param>
        /// <param name="id"></param>
        public static void RemovePlayerFromChannel(Player player, ChannelID id)
        {
            Channel channel = Channels.getChannelById(id);
            if (channel != null)
            {
                channel.RemovePlayer(player);
            }
        }

        /// <summary>
        /// Remove a player from all existing channels
        /// </summary>
        /// <param name="player"></param>
        public static void RemovePlayerFromAllChannels(Player player)
        {
            foreach (Channel channel in public_channels)
            {
                channel.RemovePlayer(player);
            }

            foreach (Channel channel in special_channels)
            {
                channel.RemovePlayer(player);
            }
        }

        /// <summary>
        /// Player opens a channel
        /// </summary>
        /// <param name="player"></param>
        /// <param name="channelId"></param>
        public static void PlayerOpenChannel(Player player, ChannelID channelId)
        {
            Channel channel = Channels.getChannelsForPlayer(player).Where(c => c.Id == channelId).FirstOrDefault();
            if (channel != null && channel.Id == channelId)
            {
                if (channel.canPlayerJoin(player))
                {
                    channel.AddPlayer(player);
                }
            }
        }

        /// <summary>
        /// Player requests channel list
        /// </summary>
        /// <param name="player"></param>
        public static void PlayerRequestChannels(Player player)
        {
            List<Channel> channels = Channels.getChannelsForPlayer(player);
            player.Connection.SendChannelList(channels);
        }

        #endregion
    }

    /// <summary>
    /// Handle In-Game existing channel
    /// </summary>
    public class Channel
    {
        #region Initializator

        /// <summary>
        /// Initialize a new channel with the given id and name
        /// </summary>
        /// <param name="newId"></param>
        /// <param name="newName"></param>
        /// <param name="newGUID"></param>
        public Channel(ChannelID newId, string newName)
        {
            Id = newId;
            Name = newName;
        }

        #endregion

        #region Properties

        private List<Player> viewers = new List<Player>();

        public ChannelID Id { get; set; }
        public string Name { get; set; }
        public List<Player> ActiveViewers { get { return viewers; } }

        #endregion

        #region Booleans

        /// <summary>
        /// Check if whether or not can the given player join this particular channel
        /// This is pretty much used to handle private/special channels
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool canPlayerJoin(Player player)
        {
            return true;
        }

        /// <summary>
        /// Check if a player has joined this channel
        /// This is pretty much an anti-packet hack
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool hasPlayerJoined(Player player)
        {
            return viewers.Contains(player);
        }

        #endregion

        #region Functioning

        /// <summary>
        /// Add a player to this channel
        /// </summary>
        /// <param name="player"></param>
        public virtual void AddPlayer(Player player)
        {
            if (!hasPlayerJoined(player))
            {
                viewers.Add(player);
                onPlayerJoinChannel(player);
            }
        }

        /// <summary>
        /// Remove a player from this channel
        /// </summary>
        /// <param name="player"></param>
        public virtual void RemovePlayer(Player player)
        {
            if (hasPlayerJoined(player))
            {
                viewers.Remove(player);
                onPlayerLeaveChannel(player);
            }
        }

        /// <summary>
        /// Player talks in the channel
        /// </summary>
        /// <param name="player"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public void PlayerTalk(Player player, TalkType type, string message)
        {
            if (hasPlayerJoined(player))
            {
                foreach (Player viewer in viewers)
                {
                    viewer.Connection.SendMessageToChannel(player, type, message, (short)Id);
                }
            }
        }

        #endregion

        #region Events

        private void onPlayerJoinChannel(Player player)
        {
            if (Id != ChannelID.RuleViolations)
            {
                player.Connection.SendChannel(this);
            }
            else
            {
                player.Connection.SendRuleViolationsChannel((short)Id);
            }
        }

        private void onPlayerLeaveChannel(Player player)
        {
            // ...
        }

        #endregion
    }
}
