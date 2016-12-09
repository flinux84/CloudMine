using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.API_server.Models
{
    public class DataChunkPartNameComparer : IComparer<DataChunk>
    {
        private static readonly string _partToken = ".part_";
        public int Compare(DataChunk x, DataChunk y)
        {
            // Extract the part after ".part_"
            var partX = x.PartName.Split(new string[] { _partToken }, StringSplitOptions.None)[1];
            var partY = y.PartName.Split(new string[] { _partToken }, StringSplitOptions.None)[1];
            
            int xNum;
            int yNum;

            // Parse the part number. The part before '.'
            int.TryParse(partX.Split('.')[0], out xNum);
            int.TryParse(partY.Split('.')[0], out yNum);

            if (xNum == yNum)
                return 0;
            if (xNum < yNum)
                return -1; 
            return 1;
        }
    }
}
