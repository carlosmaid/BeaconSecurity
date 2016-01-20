using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;

namespace JimLess
{
    class BuildHandler
    {


        public static void grid_OnBlockAdded(IMySlimBlock obj)
        {
            try
            {
                Logger.Log.Debug("BeaconSecurity.grid_OnBlockAdded {0}", obj);

                MyCubeGrid grid = obj.CubeGrid as MyCubeGrid;
                if (grid == null)
                    return;

                bool removing = false;
                if (!grid.DestructibleBlocks && Core.Settings != null && Core.Settings.BuildingNotAllowed)
                {
                    Logger.Log.Debug(" * DestructibleBlocks {0}, so block removed...", grid.DestructibleBlocks);
                    removing = true;
                }

                // check admins grids
                if (Core.Settings != null && Core.Settings.IndestructibleNoBuilds && Core.Settings.Indestructible.Contains(grid.EntityId) && !Core.Settings.IndestructibleOverrideBuilds.Contains(grid.EntityId)
                )
                {
                    Logger.Log.Debug(" * Target '{0}' in indestructible list, so block removed...", grid.DisplayName);
                    removing = true;
                }

                if (removing)
                {
                    (grid as IMyCubeGrid).RemoveBlock(obj, true);
                    if (obj.FatBlock != null)
                        obj.FatBlock.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("Exception grid_OnBlockAdded in {0}", ex.Message);
            }

        }


    }
}
