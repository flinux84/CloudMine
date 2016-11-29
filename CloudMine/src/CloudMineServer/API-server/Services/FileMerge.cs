using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Classes
{
    public class FileMerge
    {
        public bool MergeFile(string FileName)
        {

            bool rslt = false;
            // parse out the different tokens from the filename according to the convention
            string partToken = ".part_";
            string baseFileName = FileName.Substring(0, FileName.IndexOf(partToken));
            string trailingTokens = FileName.Substring(FileName.IndexOf(partToken) + partToken.Length);
            int FileIndex = 0;
            int FileCount = 0;
            int.TryParse(trailingTokens.Substring(0, trailingTokens.IndexOf(".")), out FileIndex);
            int.TryParse(trailingTokens.Substring(trailingTokens.IndexOf(".") + 1), out FileCount);

            // get a list of all file parts in the temp folder

            string Searchpattern = Path.GetFileName(baseFileName) + partToken + "*";
            string[] FilesList = Directory.GetFiles(Path.GetDirectoryName(FileName), Searchpattern);

            if (FilesList.Count() == FileCount)
            {
                // use a singleton to stop overlapping processes
                if (!MergeFileManager.Instance.InUse(baseFileName))
                {
                    MergeFileManager.Instance.AddFile(baseFileName);
                    if (File.Exists(baseFileName))
                        File.Delete(baseFileName);
                    List<SortedFile> MergeList = new List<SortedFile>();
                    foreach (string File in FilesList)
                    {
                        SortedFile sFile = new SortedFile();
                        sFile.FileName = File;
                        baseFileName = File.Substring(0, File.IndexOf(partToken));
                        trailingTokens = File.Substring(File.IndexOf(partToken) + partToken.Length);
                        int.TryParse(trailingTokens.
                           Substring(0, trailingTokens.IndexOf(".")), out FileIndex);
                        sFile.FileOrder = FileIndex;
                        MergeList.Add(sFile);
                    }
                    // sort by the file-part number to ensure we merge back in the correct order
                    var MergeOrder = MergeList.OrderBy(s => s.FileOrder).ToList();
                    using (FileStream fileStream = new FileStream(baseFileName, FileMode.Create))
                    {
                        // merge each file chunk back into one contiguous file stream
                        foreach (var chunk in MergeOrder)
                        {
                            try
                            {
                                using (FileStream fileChunk =
                                   new FileStream(chunk.FileName, FileMode.Open))
                                {
                                    fileChunk.CopyTo(fileStream);
                                }
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                    rslt = true;
                    // unlock the file from singleton
                    MergeFileManager.Instance.RemoveFile(baseFileName);
                    foreach (string x in FilesList)
                    {
                        File.Delete(x);
                    }
                }
            }
            return rslt;

        }
        public class MergeFileManager
        {
            private static MergeFileManager instance;
            private List<string> MergeFileList;

            private MergeFileManager()
            {
                try
                {
                    MergeFileList = new List<string>();
                }
                catch { }
            }

            public static MergeFileManager Instance
            {
                get
                {
                    if (instance == null)
                        instance = new MergeFileManager();
                    return instance;
                }
            }

            public void AddFile(string BaseFileName)
            {
                MergeFileList.Add(BaseFileName);
            }

            public bool InUse(string BaseFileName)
            {
                return MergeFileList.Contains(BaseFileName);
            }

            public bool RemoveFile(string BaseFileName)
            {
                return MergeFileList.Remove(BaseFileName);
            }
        }
        public struct SortedFile
        {
            public int FileOrder { get; set; }
            public String FileName { get; set; }
        }
    }
}
