using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer.Environment;

namespace GameServer.Utils
{
    /// <summary>
    /// Handle all rule violations done with (Ctrl+R)
    /// </summary>
    public static class RuleViolations
    {
        #region Properties

        private static List<Violation> rule_violations = new List<Violation>();
        public static List<Violation> ActiveRuleViolations { get { return rule_violations; } }

        #endregion

        #region Booleans

        /// <summary>
        /// Checks whether or not a certain player has reported a rule violation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool hasPlayerReportedRuleViolation(string name)
        {
            foreach (Violation violation in rule_violations)
            {
                if (violation.Player.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Get

        /// <summary>
        /// Get a player rule violation report
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Violation getPlayerRuleViolationReport(Player player)
        {
            foreach (Violation violation in rule_violations)
            {
                if (violation.Player == player)
                {
                    return violation;
                }
            }

            return null;
        }

        #endregion

        #region Functioning

        /// <summary>
        /// Gamemaster processes a rule violation report
        /// </summary>
        /// <param name="gamemaster"></param>
        /// <param name="owner"></param>
        public static void PlayerProcessRuleViolation(Player gamemaster, Player owner)
        {
            Violation violation = getPlayerRuleViolationReport(owner);
            if (violation != null)
            {
                violation.Gamemaster = gamemaster;
                violation.IsOpen = false;
                Channel ruleViolationChannel = Channels.getChannelById(ChannelID.RuleViolations);
                if (ruleViolationChannel != null)
                {
                    foreach (Player viewer in ruleViolationChannel.ActiveViewers)
                    {
                        viewer.Connection.SendRuleViolationReportRemoval(owner.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Close a player rule violation
        /// </summary>
        /// <param name="player"></param>
        public static void PlayerCloseRuleViolation(Player player)
        {
            Violation violation = getPlayerRuleViolationReport(player);
            if (violation != null)
            {
                player.Connection.SendLockRuleViolationReport();
                Channel ruleViolationChannel = Channels.getChannelById(ChannelID.RuleViolations);
                if (violation.IsOpen && ruleViolationChannel != null)
                {
                    foreach (Player viewer in ruleViolationChannel.ActiveViewers)
                    {
                        viewer.Connection.SendRuleViolationReportRemoval(player.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Player cancels his rule violation report
        /// </summary>
        /// <param name="player"></param>
        public static void CancelPlayerRuleViolation(Player player)
        {
           Violation violation = getPlayerRuleViolationReport(player);
           if (violation != null)
           {
               if (violation.Gamemaster != null)
               {
                   violation.Gamemaster.Connection.SendRuleViolationCancel(player.Name);
               }
               else
               {
                   // The report was active but not answered, so let's tell all active game masters with the channel open
                   // that the report was canceled
                   Channel ruleViolationChannel = Channels.getChannelById(ChannelID.RuleViolations);
                   if (ruleViolationChannel != null)
                   {
                       foreach (Player viewer in ruleViolationChannel.ActiveViewers)
                       {
                           viewer.Connection.SendRuleViolationReportRemoval(player.Name);
                       }
                   }
               }
               rule_violations.Remove(violation);
           }
        }

        /// <summary>
        /// Player continues in his rule violation report
        /// </summary>
        /// <param name="player"></param>
        /// <param name="message"></param>
        public static void PlayerContinueRuleViolationReport(Player player, string message)
        {
            Violation violation = getPlayerRuleViolationReport(player);
            if (violation != null)
            {
                if (violation.Gamemaster != null)
                {
                    violation.Gamemaster.Connection.SendCreatureSay(player, TalkType.RuleViolationContinue, message);
                    player.Connection.SendTextMessage((byte)MessageType.StatusSmall, "Message sent to Gamemaster.");
                }
            }
        }

        /// <summary>
        /// Player closes a rule violation report
        /// </summary>
        /// <param name="player"></param>
        public static void CloseRuleViolationReport(Player player)
        {
            Violation violation = getPlayerRuleViolationReport(player);
            if (violation != null)
            {
                rule_violations.Remove(violation);
                player.Connection.SendLockRuleViolationReport();
                Channel channel = Channels.getChannelById(ChannelID.RuleViolations);
                if (channel != null)
                {
                    foreach (Player viewer in channel.ActiveViewers)
                    {
                        viewer.Connection.SendRuleViolationReportRemoval(player.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Player reports a rule violation with Ctrl + R
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Message"></param>
        public static void PlayerReportRuleViolation(Player player, string message)
        {
            CancelPlayerRuleViolation(player);

            Violation violation = new Violation();
            violation.Player = player;
            violation.Message = message;
            violation.Time = 0;
            violation.IsOpen = true;

            rule_violations.Add(violation);

            Channel ruleViolationChannel = Channels.getChannelById(ChannelID.RuleViolations);
            if (ruleViolationChannel != null)
            {
                foreach (Player viewer in ruleViolationChannel.ActiveViewers)
                {
                    viewer.Connection.SendMessageToChannel(player, TalkType.RuleViolationChannel, message, (short)ChannelID.RuleViolations, violation.Time);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Handle a Rule Violation report (Ctrl+R)
    /// </summary>
    public class Violation
    {
        #region Initializator

        public Violation()
        {
            Player = null;
            Gamemaster = null;
            Message = null;
            Time = 0;
            IsOpen = false;
        }

        #endregion

        #region Properties

        public Player Player { get; set; }
        public Player Gamemaster { get; set; }
        public string Message { get; set; }
        public int Time { get; set; }
        public bool IsOpen { get; set; }

        #endregion
    }
}
