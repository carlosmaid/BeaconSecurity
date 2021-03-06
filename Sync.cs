﻿using Sandbox.ModAPI;
using System;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;

namespace JimLess
{
    public class Sync
    {
        public static void OnSyncRequest(byte[] bytes)
        {
            try
            {
                Logger.Log.Debug("BeaconSecurity.OnSyncRequest() - starts");

                SyncPacket pckIn = new SyncPacket();
                string data = System.Text.Encoding.Unicode.GetString(bytes);
                //Logger.Log.Debug(@"*******************************\n{0}\n*******************************\n", data);
                pckIn = MyAPIGateway.Utilities.SerializeFromXML<SyncPacket>(data);
                Logger.Log.Info("OnSyncRequest COMMAND:{0}, id:{1}, entity:'{2}', steamid: {3}, isserver: {4}", Enum.GetName(typeof(Command), pckIn.command), pckIn.ownerId, pckIn.entityId, pckIn.steamId, Core.IsServer);

                if (pckIn.proto != SyncPacket.Version)
                {
                    Logger.Log.Error("Wrong version of sync protocol client [{0}] <> [{1}] server", SyncPacket.Version, pckIn.proto);
                    MyAPIGateway.Utilities.ShowNotification("Sync Protocol version mismatch! Try to restart game or server!", 5000, MyFontEnum.Red);
                    return;
                }

                switch ((Command)pckIn.command)
                {
                    case Command.MessageToChat:
                        {
                            MyAPIGateway.Utilities.ShowMessage(Core.MODSAY, pckIn.message);
                            break;
                        }
                    case Command.SettingsSync:
                        {
                            if (pckIn.request) // Settings sync request
                            {
                                if (Core.IsServer && Core.Settings != null)
                                { // server send settings to client
                                    Logger.Log.Info("Send sync packet with settings to user steamId {0}", pckIn.steamId);
                                    SyncPacket pckOut = new SyncPacket();
                                    pckOut.proto = SyncPacket.Version;
                                    pckOut.request = false;
                                    pckOut.command = (ushort)Command.SettingsSync;
                                    pckOut.steamId = pckIn.steamId;
                                    pckOut.settings = Core.Settings;
                                    Core.SendMessage(pckOut, pckIn.steamId);
                                }
                            }
                            else
                            {
                                if (!Core.IsServer)
                                { // setting sync only for clients
                                    Logger.Log.Info("User config synced...");
                                    // if settings changes or syncs...
                                    Core.setSettings(pckIn.settings);
                                    if (pckIn.steamId == 0) // if steamid is zero, so we updating for all clients and notify this message
                                        MyAPIGateway.Utilities.ShowNotification("Beacon Security settings has been updated!", 2000, MyFontEnum.Green);
                                }
                            }
                            break;
                        }
                    case Command.SettingsChange:
                        {
                            if (Core.IsServer) // Only server can acccept this message
                            {
                                Logger.Log.Info("Some one with steamid={0} trying to change server settings", pckIn.steamId);
                                if (Core.IsAdmin(pckIn.steamId) || pckIn.steamId == MyAPIGateway.Session.Player.SteamUserId)
                                {
                                    Logger.Log.Info("Server config changed by steamId {0}", pckIn.steamId);
                                    Core.setSettings(pckIn.settings);

                                    // resend for all clients a new settings
                                    SyncPacket newpacket = new SyncPacket();
                                    newpacket.proto = SyncPacket.Version;
                                    newpacket.request = false;
                                    newpacket.command = (ushort)Command.SettingsSync;
                                    newpacket.steamId = 0; // for all
                                    newpacket.settings = Core.Settings;
                                    Core.SendMessage(newpacket);
                                }
                            }
                            break;
                        }
                    case Command.SyncOff:
                        {
                            IMyEntity entity;
                            MyAPIGateway.Entities.TryGetEntityById(pckIn.entityId, out entity);
                            if (entity == null)
                                break;

                            Logger.Log.Debug("SyncOff found entity id {0}", pckIn.entityId);
                            BeaconSecurity bs = entity.GameLogic as BeaconSecurity;
                            if (bs == null || !bs.IsBeaconSecurity)
                                break;

                            Logger.Log.Debug(" * entity is BeaconSecurity");
                            if (entity is IMyFunctionalBlock)
                                (entity as IMyFunctionalBlock).RequestEnable(false);

                            IMyPlayer player = MyAPIGateway.Session.Player;
                            if (player != null) // check this for dedicated servers
                            {
                                MyRelationsBetweenPlayerAndBlock relation = (entity as IMyFunctionalBlock).GetUserRelationToOwner(player.PlayerID);
                                if (relation == MyRelationsBetweenPlayerAndBlock.Owner || relation == MyRelationsBetweenPlayerAndBlock.FactionShare)
                                {  // if player has rights to this beacon, show message
                                    MyAPIGateway.Utilities.ShowNotification(string.Format("{0} beacon security called '{1}' is deactivated now!", (relation == MyRelationsBetweenPlayerAndBlock.FactionShare) ? "Faction" : "Your", (entity.GameLogic as BeaconSecurity).DisplayName), 5000, MyFontEnum.Red);
                                }
                            }
                            break;
                        }
                    case Command.SyncOn:
                        {
                            IMyEntity entity;
                            MyAPIGateway.Entities.TryGetEntityById(pckIn.entityId, out entity);
                            if (entity == null)
                                break;

                            Logger.Log.Debug("SyncOn found entity id {0}", pckIn.entityId);
                            BeaconSecurity bs = entity.GameLogic as BeaconSecurity;
                            if (bs == null || !bs.IsBeaconSecurity)
                                break;

                            Logger.Log.Debug(" * entity is BeaconSecurity");
                            if (entity is IMyFunctionalBlock)
                                (entity as IMyFunctionalBlock).RequestEnable(true);

                            IMyPlayer player = MyAPIGateway.Session.Player;
                            if (player != null) // check this for dedicated servers
                            {
                                MyRelationsBetweenPlayerAndBlock relation = (entity as IMyFunctionalBlock).GetUserRelationToOwner(player.PlayerID);
                                if (relation == MyRelationsBetweenPlayerAndBlock.Owner || relation == MyRelationsBetweenPlayerAndBlock.FactionShare)
                                {  // if player has rights to this beacon, show message
                                    MyAPIGateway.Utilities.ShowNotification(string.Format("{0} beacon security called '{1}' is activated now!", (relation == MyRelationsBetweenPlayerAndBlock.FactionShare) ? "Faction" : "Your", (entity.GameLogic as BeaconSecurity).DisplayName), 5000, MyFontEnum.Green);
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception at BeaconSecurity.OnSyncRequest(): {0}", ex.Message);
                return;
            }
        }
    }
}
