using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JimLess
{
    public class DamageHandler
    {
        public static void BeforeDamageHandler(object target, ref MyDamageInformation info)
        {
            try
            {
                Logger.Log.Debug("BeaconSecurity.BeforeDamageHandler {0} - {1}, {2}, {3}", target, info.Amount, info.Type, info.AttackerId);

                IMySlimBlock targetBlock = target as IMySlimBlock;
                if (targetBlock == null)
                    return;

                MyCubeGrid targetGrid = targetBlock.CubeGrid as MyCubeGrid;

                if (targetGrid != null && !targetGrid.DestructibleBlocks)
                {
                    Logger.Log.Debug(" * DestructibleBlocks {0}, so DAMAGE IGNORED...", targetGrid.DestructibleBlocks);
                    info.Amount = 0f;
                }

                // check admins grids
                if (Core.Settings != null && Core.Settings.Indestructible.Contains(targetGrid.DisplayName))
                {
                    Logger.Log.Debug(" * Target '{0}' in indestructible list, so DAMAGE IGNORED...", targetGrid.DisplayName);
                    info.Amount = 0f;
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception BeforeDamageHandler in {0}", ex.Message);
            }
        }
    }
}
