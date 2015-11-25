using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JimLess
{
    class BuildHandler
    {


        public static void grid_OnBlockAdded(IMySlimBlock obj)
        {
            try
            {
                Logger.Log.Debug("BeaconSecurity.grid_OnBlockAdded {0}", obj);

                if (Core.Settings != null && !Core.Settings.BuildingNotAllowed)
                    return; // allways can build...

                MyCubeGrid grid = obj.CubeGrid as MyCubeGrid;

                bool removing = false;
                if (grid != null && !grid.DestructibleBlocks)
                {
                    Logger.Log.Debug(" * DestructibleBlocks {0}, so block removed...", grid.DestructibleBlocks);
                    removing = true;
                }

                // check admins grids
                if (Core.Settings != null && Core.Settings.Indestructible.Contains(grid.DisplayName))
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
