using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.API_server.Services
{
    public static class DataChunkExtensions
    {
        private static readonly string _partToken = ".part_";
        public static string NextName(this DataChunk dataChunk)
        {
            int partNumber = GetPartNumber(dataChunk.PartName);
            if ((partNumber+1) > GetTotalCount(dataChunk.PartName))
                return null;
            return ReplacePartNumberWith(dataChunk.PartName, partNumber + 1);
        }
        public static string PreviousName(this DataChunk dataChunk)
        {
            return String.Empty;
        }
        public static string FirstInSequenceName(this DataChunk dataChunk)
        {
            return String.Empty;
        }
        public static string LastInSequenceName(this DataChunk dataChunk)
        {
            return String.Empty;
        }
        public static int Index(this DataChunk dataChunk)
        {
            return 0;
        }
        public static int NumberOfChunksInSequence(this DataChunk dataChunk)
        {
            return 0;
        }
        public static bool IsLastInSequence(this DataChunk dataChunk)
        {
            return false;
        }

        private static int GetPartNumber(string partName)
        {
            var partPart = partName.Split(new string[] { _partToken }, StringSplitOptions.None)[1];
            int part = 0;
            int.TryParse(partPart.Split('.')[0], out part);
            return part;
        }
        private static int GetTotalCount(string partName)
        {
            var partPart = partName.Split(new string[] { _partToken }, StringSplitOptions.None)[1];
            int count = 0;
            int.TryParse(partPart.Split('.')[1], out count);
            return count;
        }
        private static string ReplacePartNumberWith(string partName, int newPartNumber)
        {
            var firstPart = partName.Split(new string[] { _partToken }, StringSplitOptions.None)[0];
            var lastPart = partName.Split(new string[] { _partToken }, StringSplitOptions.None)[1];
            var newlastPart = 
                lastPart.Replace(
                    $"{GetPartNumber(partName)}.{GetTotalCount(partName)}",
                    $"{newPartNumber.ToString()}.{GetTotalCount(partName)}");
            return firstPart + _partToken + newlastPart;
        }
    }
}
