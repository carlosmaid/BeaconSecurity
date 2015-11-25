﻿using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.ModAPI;
using VRageMath;

namespace JimLess
{
    public class ChatHandler
    {
        #region TextConstants
        private static readonly string BSHELP = @"/bs help [1-9] - Show this page or explanation of variables
/bs config  - Show current settings
admin commands:
/bs on/off - Enable / disable this mod
/bs debug - Enable / disable this debug
/bs set number/name value - Set variable, you can use numbers and on/off, true/false
/bs find/list/add/rem/clear - Find,List,Add,Remove,Clear of indestructible grids
examples:
 /bs set 1 1 or /bs set DelayBeforeTurningOn 1
 /bs add Platform 3534 or /bs find 100 and /bs add

@ 2015 JimLess 
";
        private static readonly string BSHELP_CHAT = @"/bs help [1-9], /bs config, /bs set number/name value,  /bs on/off, /bs debug, /bs list/find/add/rem/clear";
        private static readonly List<string> BSHELP_VARIABLES = new List<string>() {
"1) DelayBeforeTurningOn(0-3600) - The delay before turning on protection in seconds. This time the owner should not be in the zone of action Beacon Security. Default: 120",
"2) DistanceBeforeTurningOn(0-10000) - The distance at which no owner to be found. Or the player can simply leave the game. Default: 400",
"3) OnlyForStations(on/off) - Turn on/off limitation of the Beacon Security only for stations. Default: off",
"4) OnlyWithZeroSpeed(on/off) -  If this option is enabled, a Beacon Security works only on grid with zero speed. Default: on",
"5) BuildingNotAllowed(on/off) - Turn on/off the ability to build in the area of the Beacon Security. Default: on",
"6) LimitGridSizes(0-1000) - Limitation on the sizes for grid in meters. If there are excess size, the Beacon Security would't work. Default: 150. 0 - disabled",
"7) LimitPerFaction(1-100) - Limitation on the number of Beacon Security per faction. Default: 30",
"8) LimitPerPlayer(1-100) - Limitation on the number of Beacon Security pers player. Default: 3",
"9) CleaningFrequency(0-3600) - How often is the cleaning in seconds. Default: 5. 0 - disabled"
        };
        #endregion TextConstants

        private static List<string> m_lastFound = new List<string>();
        public static void ChatMessageEntered(string messageText, ref bool sendToOthers)
        {
            Logger.Log.Debug("ChatEvents.ChatMessageEntered: {0}", messageText);
            if (messageText.StartsWith("/bs", StringComparison.OrdinalIgnoreCase))
            {
                if (messageText.Length == 3)
                {
                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, String.Format("Beacon Security is {0}. {1} Current version: {2}", (Core.Settings.Enabled) ? "On" : "Off", (Core.Settings.OnlyForStations) ? "Only for stations." : "", SyncPacket.Version));
                }
                string[] commands = (messageText.Remove(0, 3)).Split(null as string[], 2, StringSplitOptions.RemoveEmptyEntries);
                if (commands.Length > 0)
                {
                    string internalCommand = commands[0];
                    string arguments = (commands.Length > 1) ? commands[1] : "";
                    Logger.Log.Debug("internalCommand: {0} arguments {1}", internalCommand, arguments);

                    if (internalCommand.Equals("help", StringComparison.OrdinalIgnoreCase))
                    {
                        #region help
                        var index = 0;
                        try { index = Int32.Parse(arguments); }
                        catch { }
                        if (index < 1 || index > 9)
                        {
                            MyAPIGateway.Utilities.ShowMissionScreen("Help", "Beacon ", "Security", BSHELP);
                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, BSHELP_CHAT);
                        }
                        else
                        {
                            MyAPIGateway.Utilities.ShowMissionScreen("Help: explanation of variables", "Beacon ", "Security", BSHELP_VARIABLES[index - 1]);
                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, BSHELP_VARIABLES[index - 1]);
                        }
                        #endregion help
                    }
                    else if (Core.Settings != null)
                    {

                        if (internalCommand.Equals("config", StringComparison.OrdinalIgnoreCase))
                        {
                            #region config
                            MyAPIGateway.Utilities.ShowMissionScreen("Config", "Beacon ", "Security", String.Format(@"1) DelayBeforeTurningOn(0-3600,D:120) = {0}
2) DistanceBeforeTurningOn(0-10000,D:400) = {1}
3) OnlyForStations(on/off,D:off) = {2}
4) OnlyWithZeroSpeed(on/off,D:on) = {3}
5) BuildingNotAllowed(on/off,D:on) = {4}
6) LimitGridSizes(0-1000,D:150) = {5}
7) LimitPerFaction(1-100,D:30) = {6}
8) LimitPerPlayer(1-100,D:3) = {7}
9) CleaningFrequency(0-3600,D:5) = {8}
",
    Core.Settings.DelayBeforeTurningOn,
    Core.Settings.DistanceBeforeTurningOn,
    Core.Settings.OnlyForStations,
    Core.Settings.OnlyWithZeroSpeed,
    Core.Settings.BuildingNotAllowed,
    Core.Settings.LimitGridSizes,
    Core.Settings.LimitPerFaction,
    Core.Settings.LimitPerPlayer,
    Core.Settings.CleaningFrequency
    ));
                            #endregion config
                        }

                        if (Core.IsServer || Core.IsAdmin(MyAPIGateway.Session.Player))
                        {
                            #region alladminsettings
                            if (internalCommand.Equals("on", StringComparison.OrdinalIgnoreCase))
                            {
                                Core.Settings.Enabled = true;
                                Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                SyncPacket newpacket = new SyncPacket();
                                newpacket.proto = SyncPacket.Version;
                                newpacket.command = (ushort)Command.MessageToChat;
                                newpacket.message = "Beacon Security is ON.";
                                Core.SendMessage(newpacket); // send to others
                            }
                            else if (internalCommand.Equals("off", StringComparison.OrdinalIgnoreCase))
                            {
                                Core.Settings.Enabled = false;
                                Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                SyncPacket newpacket = new SyncPacket();
                                newpacket.proto = SyncPacket.Version;
                                newpacket.command = (ushort)Command.MessageToChat;
                                newpacket.message = "Beacon Security is OFF.";
                                Core.SendMessage(newpacket); // send to others
                            }
                            else if (internalCommand.Equals("debug", StringComparison.OrdinalIgnoreCase))
                            {
                                Core.Settings.Debug = !Core.Settings.Debug;
                                Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Debug logging is {0}", (Core.Settings.Debug) ? "ON" : "OFF"));
                            }
                            else if (internalCommand.Equals("list", StringComparison.OrdinalIgnoreCase))
                            {
                                string list = String.Join(", ", Core.Settings.Indestructible.ToArray());
                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Indestructible list: {0}", (Core.Settings.Indestructible.Count > 0) ? list : "[EMPTY]"));
                            }
                            else if (internalCommand.Equals("add", StringComparison.OrdinalIgnoreCase))
                            {
                                if (arguments.Length > 0)
                                {
                                    if (arguments.Length < 48)
                                    {
                                        if (Core.Settings.Indestructible.Contains(arguments))
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Can't add grid name '{0}', already in list...", arguments));
                                        }
                                        else
                                        {
                                            Core.Settings.Indestructible.Add(arguments);
                                            Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Grid name '{0}' added.", arguments));
                                        }
                                    }
                                    else
                                    {
                                        MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Error... Length of grid name limited to 48 chars."));
                                    }
                                }
                                else
                                {
                                    List<string> added = new List<string>();
                                    foreach (string gridname in m_lastFound)
                                    {

                                        if (!Core.Settings.Indestructible.Contains(gridname))
                                        {
                                            added.Add(gridname);
                                            Core.Settings.Indestructible.Add(gridname);
                                        }
                                    }

                                    string list = String.Join(", ", added.ToArray());
                                    Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);
                                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Added {0} grid names: {1}", added.Count, list));
                                }
                            }
                            else if (internalCommand.Equals("rem", StringComparison.OrdinalIgnoreCase) || internalCommand.Equals("del", StringComparison.OrdinalIgnoreCase))
                            {
                                if (arguments.Length > 0)
                                {

                                    if (!Core.Settings.Indestructible.Contains(arguments))
                                    {
                                        MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Grid name '{0}' not found in list...", arguments));
                                    }
                                    else
                                    {
                                        Core.Settings.Indestructible.Remove(arguments);
                                        Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                        MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Grid name '{0}' removed.", arguments));
                                    }
                                }
                                else
                                {
                                    List<string> removed = new List<string>();
                                    foreach (string gridname in m_lastFound)
                                    {

                                        if (Core.Settings.Indestructible.Contains(gridname))
                                        {
                                            removed.Add(gridname);
                                            Core.Settings.Indestructible.Remove(gridname);
                                        }
                                    }

                                    string list = String.Join(", ", removed.ToArray());
                                    Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);
                                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Removed {0} grid names: {1}", removed.Count, list));
                                }
                            }
                            else if (internalCommand.Equals("clear", StringComparison.OrdinalIgnoreCase))
                            {
                                Core.Settings.Indestructible.Clear();
                                Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, "Indestructible list cleared.");
                            }
                            else if (internalCommand.Equals("find", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    uint value = (arguments.Length > 0) ? UInt32.Parse(arguments) : 10;
                                    if (value < 1)
                                        value = 1;
                                    if (value > 100000)
                                        value = 100000;

                                    Vector3D pos = MyAPIGateway.Session.Player.GetPosition();
                                    BoundingSphereD sphere = new BoundingSphereD(pos, value);
                                    List<IMyEntity> entities = MyAPIGateway.Entities.GetEntitiesInSphere(ref sphere);

                                    string[] found = entities.FindAll(x => x is IMyCubeGrid).Select(x => (x as IMyCubeGrid).DisplayName).ToArray();
                                    string foundedList = String.Join(", ", found);
                                    m_lastFound.Clear();
                                    m_lastFound.AddRange(found);
                                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("In radius {0}m found: {1}", value, foundedList));
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log.Error("Exception in command find: {0}", ex.Message);
                                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Exception: {0}", ex.Message));
                                }
                            }
                            else if (internalCommand.Equals("findadd", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    uint value = UInt16.Parse(arguments);
                                    if (value < 1)
                                        value = 1;
                                    if (value > 100000)
                                        value = 100000;
                                    Vector3D pos = MyAPIGateway.Session.Player.GetPosition();
                                    BoundingSphereD sphere = new BoundingSphereD(pos, value);
                                    List<IMyEntity> entities = MyAPIGateway.Entities.GetEntitiesInSphere(ref sphere);

                                    string str = String.Join(", ", entities.FindAll(x => x is IMyCubeGrid).Select(x => (x as IMyCubeGrid).DisplayName).ToArray());
                                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("In radius {0}m found: {1}", value, str));
                                }
                                catch (Exception ex)
                                {
                                    Logger.Log.Error("Exception in command find: {0}", ex.Message);
                                }
                            }
                            else if (internalCommand.Equals("set", StringComparison.OrdinalIgnoreCase))
                            {
                                #region setcommand
                                string ResultMessage = "";
                                string[] argument = (arguments).Split(null as string[], 2, StringSplitOptions.RemoveEmptyEntries);

                                if (argument.Length >= 2)
                                {
                                    bool changed = false;
                                    if (argument[0].Equals("1") || argument[0].Equals("DelayBeforeTurningOn", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            ushort value = UInt16.Parse(argument[1]);
                                            if (value < 0 || value > 3600)
                                            {
                                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("The value is not within the allowed limits. [ 0 - 3600 ]", value));
                                            }
                                            else
                                            {
                                                Core.Settings.DelayBeforeTurningOn = value;
                                                changed = true;
                                                ResultMessage = string.Format("DelayBeforeTurningOn changed to {0}", value);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Incorrect number. {0}", ex.Message));
                                        }
                                    }
                                    else if (argument[0].Equals("2") || argument[0].Equals("DistanceBeforeTurningOn", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            ushort value = UInt16.Parse(argument[1]);
                                            if (value < 0 || value > 10000)
                                            {
                                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("The value is not within the allowed limits. [ 0 - 10000 ]", value));
                                            }
                                            else
                                            {
                                                Core.Settings.DistanceBeforeTurningOn = value;
                                                changed = true;
                                                ResultMessage = string.Format("DistanceBeforeTurningOn changed to {0}", value);

                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Incorrect number. {0}", ex.Message));
                                        }
                                    }
                                    else if (argument[0].Equals("3") || argument[0].Equals("OnlyForStations", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (argument[1].Equals("on", StringComparison.OrdinalIgnoreCase) || argument[1].Equals("true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Core.Settings.OnlyForStations = true;
                                            changed = true;
                                        }
                                        else if (argument[1].Equals("off", StringComparison.OrdinalIgnoreCase) || argument[1].Equals("false", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Core.Settings.OnlyForStations = false;
                                            changed = true;
                                        }
                                        ResultMessage = string.Format("OnlyForStations changed to {0}", (Core.Settings.OnlyForStations) ? "On" : "Off");
                                    }
                                    else if (argument[0].Equals("4") || argument[0].Equals("OnlyWithZeroSpeed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (argument[1].Equals("on", StringComparison.OrdinalIgnoreCase) || argument[1].Equals("true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Core.Settings.OnlyWithZeroSpeed = true;
                                            changed = true;
                                        }
                                        else if (argument[1].Equals("off", StringComparison.OrdinalIgnoreCase) || argument[1].Equals("false", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Core.Settings.OnlyWithZeroSpeed = false;
                                            changed = true;
                                        }
                                        ResultMessage = string.Format("OnlyWithZeroSpeed changed to {0}", (Core.Settings.OnlyWithZeroSpeed) ? "On" : "Off");
                                    }
                                    else if (argument[0].Equals("5") || argument[0].Equals("BuildingNotAllowed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        if (argument[1].Equals("on", StringComparison.OrdinalIgnoreCase) || argument[1].Equals("true", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Core.Settings.BuildingNotAllowed = true;
                                            changed = true;
                                        }
                                        else if (argument[1].Equals("off", StringComparison.OrdinalIgnoreCase) || argument[1].Equals("false", StringComparison.OrdinalIgnoreCase))
                                        {
                                            Core.Settings.BuildingNotAllowed = false;
                                            changed = true;
                                        }
                                        ResultMessage = string.Format("BuildingNotAllowed changed to {0}", (Core.Settings.BuildingNotAllowed) ? "On" : "Off");
                                    }
                                    else if (argument[0].Equals("6") || argument[0].Equals("LimitGridSizes", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            ushort value = UInt16.Parse(argument[1]);
                                            if (value < 0 || value > 1000)
                                            {
                                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("The value is not within the allowed limits. [ 0 - 1000 ]", value));
                                            }
                                            else
                                            {
                                                Core.Settings.LimitGridSizes = value;
                                                changed = true;
                                                ResultMessage = string.Format("LimitGridSizes changed to {0}", value);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Incorrect number. {0}", ex.Message));
                                        }
                                    }
                                    else if (argument[0].Equals("7") || argument[0].Equals("LimitPerFaction", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            ushort value = UInt16.Parse(argument[1]);
                                            if (value < 1 || value > 100)
                                            {
                                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("The value is not within the allowed limits. [ 1 - 100 ]", value));
                                            }
                                            else
                                            {
                                                Core.Settings.LimitPerFaction = value;
                                                changed = true;
                                                ResultMessage = string.Format("LimitPerFaction changed to {0}", value);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Incorrect number. {0}", ex.Message));
                                        }
                                    }
                                    else if (argument[0].Equals("8") || argument[0].Equals("LimitPerPlayer", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            ushort value = UInt16.Parse(argument[1]);
                                            if (value < 1 || value > 100)
                                            {
                                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("The value is not within the allowed limits. [ 1 - 100 ]", value));
                                            }
                                            else
                                            {
                                                Core.Settings.LimitPerPlayer = value;
                                                changed = true;
                                                ResultMessage = string.Format("LimitPerPlayer changed to {0}", value);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Incorrect number. {0}", ex.Message));
                                        }
                                    }
                                    else if (argument[0].Equals("9") || argument[0].Equals("CleaningFrequency", StringComparison.OrdinalIgnoreCase))
                                    {
                                        try
                                        {
                                            ushort value = UInt16.Parse(argument[1]);
                                            if (value < 0 || value > 3600)
                                            {
                                                MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("The value is not within the allowed limits. [ 0 - 3600 ]", value));
                                            }
                                            else
                                            {
                                                Core.Settings.CleaningFrequency = value;
                                                changed = true;
                                                ResultMessage = string.Format("CleaningFrequency changed to {0}", value);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, string.Format("Incorrect number. {0}", ex.Message));
                                        }
                                    }

                                    if (changed)
                                    {
                                        Core.SendSettingsToServer(Core.Settings, MyAPIGateway.Session.Player.SteamUserId);

                                        SyncPacket newpacket = new SyncPacket();
                                        newpacket.proto = SyncPacket.Version;
                                        newpacket.command = (ushort)Command.MessageToChat;
                                        newpacket.message = ResultMessage;
                                        Core.SendMessage(newpacket); // send to others
                                    }
                                    if (!Core.IsServer)
                                        MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, ResultMessage);
                                }
                                else
                                {
                                    MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, "Incorrect syntax for command set. Use: /bs set var value");
                                }
                                #endregion setcommand
                            }
                            #endregion alladminsettings
                        }
                        else
                        {
                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, "Only an administrator can change the settings...");
                        }
                    }
                }
                sendToOthers = false;
            }
        }

    }
}