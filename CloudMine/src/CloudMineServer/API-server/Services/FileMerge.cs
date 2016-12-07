using CloudMineServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CloudMineServer.Services
{
    /// <summary>
    /// Takes a FileItem with datachunks and restores them to a file and returns path for the file
    /// </summary>
    public class FileMerge
    {
        private const string partToken = ".part_";
        private string trailingToken = "";
        private int fileIndex = 0;

        public Uri MakeFile(FileItem fileitem)
        {
            var baseFileName = fileitem.FileName;

            if (!fileitem.DataChunks.Any())
            {
                throw new InvalidOperationException("Can not merge without datachunks");
            }

            string[] filesList = fileitem.DataChunks.Select(d => d.PartName).ToArray();
            
            if (!fileitem.DataChunks.FirstOrDefault().PartName.Contains(baseFileName))
            {
                throw new InvalidOperationException("chunkname does not match filename");
            }

            var chunkPartName = filesList.FirstOrDefault();
            trailingToken = chunkPartName.Substring(chunkPartName.IndexOf(partToken) + partToken.Length);

            int fileCount = 0;
            int.TryParse(trailingToken.Substring(trailingToken.IndexOf(".") + 1), out fileCount);

            if (filesList.Count() == fileCount)
            {
                // Singleton så att den inte kan överlappa merge
                if (!MergeFileManager.Instance.InUse(baseFileName))
                {
                    MergeFileManager.Instance.AddFile(baseFileName);

                    //Sortera med index så vi kan merga ihop rätt
                    List<SortedFile> MergeOrder = SortMergeList(filesList);

                    //Skapa filen igen genom att merga ihop chunksen till en filestream som skrivs till Temp-mappen
                    FileInfo fi = MergeSortedChunks(fileitem, baseFileName, MergeOrder);

                    //Frigör MergeFileManager och ta bort från listan
                    MergeFileManager.Instance.RemoveFile(baseFileName);
                    foreach (string x in filesList)
                    {
                        File.Delete(x);
                    }

                    //Returnera url till filen på servern
                    return new Uri(fi.FullName);
                }
            }
            return new Uri("error");
        }

        private static FileInfo MergeSortedChunks(FileItem fileitem, string baseFileName, List<SortedFile> MergeOrder)
        {
            FileInfo fi = new FileInfo("Temp/" + baseFileName);

            using (FileStream fileStream = fi.Create())
            {
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
            return fi;
        }

        // sorterar chunks så vi har dem i rätt ordning innan vi klistrar ihop
        private List<SortedFile> SortMergeList(string[] filesList)
        {
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

            var MergeOrder = mergeList.OrderBy(s => s.FileOrder).ToList();
            return MergeOrder;
        }

        //Singleton-manager som håller koll på aktiva merges.
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


//TODO: Linus: välj in-memory merge eller disk-merge beroende på filstorlek.
//TODO: Linus: kasta exception om saknade chunks, eller namnet inte stämmer överens med fileitem.FileNamn.