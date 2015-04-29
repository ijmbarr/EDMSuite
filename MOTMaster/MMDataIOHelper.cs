using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Globalization;

using System.Diagnostics;


namespace MOTMaster
{
    public class MMDataIOHelper
    {
        MMDataZipper zipper = new MMDataZipper();
        private string motMasterDataPath;
        private string element;

        public MMDataIOHelper(string motMasterDataPath, string element)
        {
            this.motMasterDataPath = motMasterDataPath;
            this.element = element;
        }



        public void StoreRun(string saveFolder, int batchNumber, string pathToPattern, string pathToHardwareClass,
            Dictionary<String, Object> dict, Dictionary<String, Object> report,
            string cameraAttributesPath, ushort[][,] imageData, string EID,
            Dictionary<String, Object> analysisReport)
        {
            string fileTag = EID;
            string saveSubfolder = checkSaveFolder(saveFolder);

            saveToFiles(fileTag, saveSubfolder, batchNumber, pathToPattern, pathToHardwareClass, dict, report, cameraAttributesPath, imageData, analysisReport);

            string[] files = putCopiesOfFilesToZip(saveSubfolder, fileTag);

            //deleteFiles(saveFolder, fileTag);
            deleteFiles(files);

        }

        #region zipping files
        private void deleteFiles(string[] files)
        {
            foreach (string s in files)
            {
                File.Delete(s);
            }
        }

        private string[] putCopiesOfFilesToZip(string saveFolder, string fileTag)
        {

            string[] files = Directory.GetFiles(saveFolder, fileTag + "*");
            System.IO.FileStream fs = new FileStream(saveFolder + fileTag + ".zip", FileMode.Create);
            zipper.PrepareZip(fs);
            foreach (string s in files)
            {
                string[] bits = (s.Split('\\'));
                string name = bits[bits.Length - 1];
                zipper.AppendToZip(saveFolder, name);
            }
 
            zipper.CloseZip();
            fs.Close();
            return files;
        }

        #endregion

        private void saveToFiles(string fileTag, string saveFolder, int batchNumber, string pathToPattern, string pathToHardwareClass,
            Dictionary<String, Object> dict, Dictionary<String, Object> report,
            string cameraAttributesPath, ushort[][,] imageData, Dictionary<String, Object> analysisReport)
        {
            storeDictionary(saveFolder + fileTag + "_parameters.txt", dict);
            File.Copy(pathToPattern, saveFolder + fileTag + "_script.cs");
            File.Copy(pathToHardwareClass, saveFolder + fileTag + "_hardwareClass.cs");
            storeCameraAttributes(saveFolder + fileTag + "_cameraParameters.txt", cameraAttributesPath);
            storeImage(saveFolder + fileTag, imageData);
            storeDictionary(saveFolder + fileTag + "_hardwareReport.txt", report);
            storeAnalysis(saveFolder, fileTag, analysisReport);
        }

        public string SelectSavedScriptPathDialog()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DataSets|*.zip";
            dialog.Title = "Load previously saved pattern";
            dialog.Multiselect = false;
            dialog.InitialDirectory = motMasterDataPath;
            dialog.ShowDialog();
            return dialog.FileName;
        }

        public void UnzipFolder(string path)
        {
            zipper.Unzip(path);
        }

        public Dictionary<string, object> LoadDictionary(string dictionaryPath)
        {
            string[] parameterStrings = File.ReadAllLines(dictionaryPath);
            Dictionary<string, object> dict = new Dictionary<string, object>();
            char separator = '\t';
            foreach (string str in parameterStrings)
            {
                string[] keyValuePairs = str.Split(separator);
                Type t = System.Type.GetType(keyValuePairs[2]);
                dict.Add(keyValuePairs[0], Convert.ChangeType(keyValuePairs[1], t));
            }
            return dict;
        }

        public void DisposeReplicaScript(string folderPath)
        {
            Directory.Delete(folderPath, true);
        }

        #region store specific stuff
        private void storeCameraAttributes(string savePath, string attributesPath)
        {
            File.Copy(attributesPath, savePath);
        }

        private void storeImage(string savePath, ushort[][,] imageData)
        {
            for (int i = 0; i < imageData.Length; i++)
            {
                storeImage(savePath + "_" + i.ToString(), imageData[i]);
            }
        }

        private void storeImage(string savePath, ushort[,] imageData)
        {
            if (imageData == null) //Kill it if no images
            {
                return;
            }

            int width = imageData.GetLength(1);
            int height = imageData.GetLength(0);

            ushort[] pixels = imageData.Cast<ushort>().ToArray();
 
            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray16, null);
            bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixels, 2*width, 0);

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var stream = System.IO.File.Create(savePath + ".png"))
            {
                encoder.Save(stream);
            }

        }

        private void storeDictionary(String dataStoreFilePath, Dictionary<string, object> dict)
        {
            TextWriter output = File.CreateText(dataStoreFilePath);
            foreach (KeyValuePair<string, object> pair in dict)
            {
                output.Write(pair.Key);
                output.Write('\t');
                output.Write(pair.Value.ToString());
                output.Write('\t');
                output.WriteLine(pair.Value.GetType());
            }
            output.Close();


        }

        public void storeAnalysis(string filepath, string fileNameHeader, Dictionary<string, object> dict)
        {
            Dictionary<string, object> otherDict = new Dictionary<string, object>();

            foreach(KeyValuePair<string,object> pair in dict){
                
                if(pair.Value.GetType() == typeof(ushort[,])){
                    storeImage(filepath + fileNameHeader + "_" + pair.Key, (ushort[,])pair.Value);
                }
                else
                {
                    otherDict.Add(pair.Key, pair.Value);
                }
            }

            if(otherDict.Count > 0){
                storeDictionary(filepath + fileNameHeader + "_analysis.txt", otherDict);
            }
            
        }

        #endregion

        #region Manipulating filename/file path
        private string getDataID(string element, int batchNumber)
        {
            DateTime dt = DateTime.Now;
            string dateTag;
            string batchTag;
            int subTag = 0;

            dateTag = String.Format("{0:ddMMMyy}", dt);
            batchTag = batchNumber.ToString().PadLeft(2, '0');
            subTag = (Directory.GetFiles(motMasterDataPath, element +
                dateTag + batchTag + "*.zip")).Length;
            string id = element + dateTag + batchTag
                + "_" + subTag.ToString().PadLeft(3, '0');
            return id;
        }

        string checkSaveFolder(string saveFolder)
        {
            string day = DateTime.Now.ToString("dd");
            string month = DateTime.Now.ToString("MM");
            string year = DateTime.Now.ToString("yyyy");

            if (!Directory.Exists(saveFolder + "\\" + year))
            {
                Directory.CreateDirectory(saveFolder + "\\" + year);
            }

            if (!Directory.Exists(saveFolder + "\\" + year + "\\" + month))
            {
                Directory.CreateDirectory(saveFolder + "\\" + year + "\\" + month);
            }

            if (!Directory.Exists(saveFolder + "\\" + year + "\\" + month + "\\" + day))
            {
                Directory.CreateDirectory(saveFolder + "\\" + year + "\\" + month + "\\" + day);
            }

            return saveFolder + "\\" + year + "\\" + month + "\\" + day + "\\";
        }

        public string EID2Path(string basePath, string EID)
        {
            DateTime EIDTime = DateTime.ParseExact(EID, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            string day = EIDTime.ToString("dd");
            string month = EIDTime.ToString("MM");
            string year = EIDTime.ToString("yyyy");
            return basePath + "\\" + year + "\\" + month + "\\" + day + "\\";
        }
        #endregion

    }
}
