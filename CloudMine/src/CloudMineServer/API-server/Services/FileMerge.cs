using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CloudMineServer.Classes
{

    /// <summary>
    /// Takes a FileItem with datachunks and restores them to a file for download
    /// </summary>
    public class FileMerge
    {
        private const string partToken = ".part_";
        private string trailingToken = "";
        private int fileIndex = 0;

        public Uri MakeFileForDownload(FileItem fileitem)
        {
            var baseFileName = fileitem.FileName;
            string[] filesList = fileitem.DataChunks.Select(d => d.PartName).ToArray();

            var chunkPartName = filesList.FirstOrDefault();
            trailingToken = chunkPartName.Substring(chunkPartName.IndexOf(partToken) + partToken.Length);

            //kolla så att filnamnet i fileitem och datachunk är samma fil, annars kasta exception
            //TODO: Linus: välj in-memory merge eller disk-merge beroende på filstorlek.

            int fileCount = 0;
            int.TryParse(trailingToken.Substring(trailingToken.IndexOf(".") + 1), out fileCount);

            if (filesList.Count() == fileCount)
            {

                // Singleton så att den inte kan överlappa merge
                if (!MergeFileManager.Instance.InUse(baseFileName))
                {
                    MergeFileManager.Instance.AddFile(baseFileName);

                    List<SortedFile> mergeList = new List<SortedFile>();

                    foreach (var file in filesList)
                    {
                        trailingToken = file.Substring(file.IndexOf(partToken) + partToken.Length);
                        int.TryParse(trailingToken.Substring(0, trailingToken.IndexOf(".")), out fileIndex);

                        SortedFile sFile = new SortedFile();
                        sFile.FileName = file;
                        sFile.FileOrder = fileIndex;
                        mergeList.Add(sFile);
                    }

                    // sorterar chunks så vi har dem i rätt ordning innan vi klistrar ihop
                    var MergeOrder = mergeList.OrderBy(s => s.FileOrder).ToList();

                    FileInfo fi = new FileInfo(baseFileName);
                                      
                    using (FileStream fileStream = fi.Create())
                    {
                        // merge each file chunk back into one contiguous file stream
                        foreach (var chunk in MergeOrder)
                        {
                            var data = fileitem.DataChunks.FirstOrDefault(c => c.PartName == chunk.FileName);
                            try
                            {
                                using (MemoryStream fileChunk = new MemoryStream(data.Data))
                                {

                                    fileChunk.CopyTo(fileStream);
                                }
                            } catch
                            {
                                throw;
                            }
                        }

                    }

                    
                    MergeFileManager.Instance.RemoveFile(baseFileName);
                    foreach (string x in filesList)
                    {
                        File.Delete(x);
                    }
                }
            }

            return new Uri("~/download/");
        }


        public bool MergeFile(string filepartname)
        {

            bool rslt = false;
            // parse out the different tokens from the filename according to the convention
            string partToken = ".part_";
            string baseFileName = filepartname.Substring(0, filepartname.IndexOf(partToken));
            string trailingTokens = filepartname.Substring(filepartname.IndexOf(partToken) + partToken.Length);
            int FileIndex = 0;
            int FileCount = 0;
            int.TryParse(trailingTokens.Substring(0, trailingTokens.IndexOf(".")), out FileIndex);
            int.TryParse(trailingTokens.Substring(trailingTokens.IndexOf(".") + 1), out FileCount);

            // get a list of all file parts in the temp folder

            string Searchpattern = Path.GetFileName(baseFileName) + partToken + "*";
            string[] FilesList = Directory.GetFiles(Path.GetDirectoryName(filepartname), Searchpattern);

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
                            var x = chunk;

                            try
                            {
                                using (FileStream fileChunk = new FileStream(chunk.FileName, FileMode.Open))
                                {

                                    fileChunk.CopyTo(fileStream);
                                }
                            } catch
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
                } catch { }
            }

            public static MergeFileManager Instance {
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
