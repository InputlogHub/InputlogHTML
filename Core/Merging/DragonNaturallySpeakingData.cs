using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using InputLog.Core.Events.DragonNS;
using System.Threading;

namespace InputLog.Core.Merging
{
    public class DragonNaturallySpeakingData
    {
        public Dictionary<string, DragonNSPart> Entries;
        public string[] Guids;
        private string OutPath;
        private string WaveDir;
        private string FileBase;
        private int Position;
        private string DNSExporterFolder;

        public DragonNaturallySpeakingData(string filePath, string waveDir)
        {
            WaveDir = waveDir;
            Position = 0;
            OutPath = Path.GetTempPath() + "IL_DNS_" + (Guid.NewGuid()).ToString();
            String args = String.Format("-s -x txt,nwv,corr,all \"{0}\" \"{1}\"", filePath, OutPath);
            Guids = new string[] { };
            if (!TryVOld(filePath, args))
                TryVNew(filePath, args);
        }

        private bool TryVNew(string filePath, string args)
        {
            try
            {
                DNSExporterFolder = Path.Combine(Directory.GetCurrentDirectory(), "DNSNew");
                SetPath();
                Process p = NewProcess(Path.Combine(DNSExporterFolder,"DNSExporter.exe"), args);
                p.Start();
                p.WaitForExit();
                if (!Directory.Exists(OutPath)) return false;
                Entries = new Dictionary<string, DragonNSPart>();
                while (!File.Exists(OutPath + "/versioninfo.txt")) { System.Threading.Thread.Sleep(100); }
                FileBase = Path.GetFileNameWithoutExtension(filePath);
                //string data = File.ReadAllText(OutPath + "/utts_" + FileBase + "_dat.txt").Trim();
                string dPath = OutPath + "/utts_" + FileBase + "_dat.utt.txt";
                string data = File.ReadAllText(dPath).Trim();
                string[] lines = data.Split('\n');
                Guids = new string[lines.Length];
                foreach (string line in lines)
                {
                    ReadEntry(line);
                }
                string corrPath = OutPath + "/utts_" + "corr" + "_dat.txt";
                if (File.Exists(corrPath))
                {
                    string corrs = File.ReadAllText(corrPath).Trim();
                    foreach (string line in corrs.Split('\n'))
                    {
                        ReadCorrection(line);
                    }
                }
                Position = 0;
                Directory.Delete(OutPath, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool TryVOld(string filePath, string args)
        {
            try
            {
                DNSExporterFolder = Path.Combine(Directory.GetCurrentDirectory(), "DNSOld");
                //SetPath();
                Process p = NewProcess(Path.Combine(DNSExporterFolder, "DNSExporter.exe"), args);
                p.Start();
                p.WaitForExit();
                if (!Directory.Exists(OutPath)) return false;
                Entries = new Dictionary<string, DragonNSPart>();
                while (!File.Exists(OutPath + "/versioninfo.txt")) { System.Threading.Thread.Sleep(100); }
                FileBase = Path.GetFileNameWithoutExtension(filePath);
                string dPath = OutPath + "/utts_" + FileBase + "_dat.txt";
                string data = File.ReadAllText(dPath).Trim();
                string[] lines = data.Split('\n');
                Guids = new string[lines.Length];
                foreach (string line in lines)
                {
                    ReadEntryOld(line);
                }
                string corrPath = OutPath + "/utts_" + "corr" + "_dat.txt";
                if (File.Exists(corrPath))
                {
                    string corrs = File.ReadAllText(corrPath).Trim();
                    foreach (string line in corrs.Split('\n'))
                    {
                        ReadCorrectionOld(line);
                    }
                }
                Position = 0;
                Directory.Delete(OutPath, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void ReadCorrectionOld(string line)
        {
            ReadCorrection(line);
        }

        private void SetPath()
        {
            Process p1 = NewProcess("cmd.exe", "set USE_EXTERNAL_DFILE=1");
            p1.Start();
            //p1.WaitForExit();
            Process p2 = NewProcess("cmd.exe", "set PATH=%PATH%;" + DNSExporterFolder);
            p2.Start();
            //p2.WaitForExit();
        }

        public DragonNSPart Next()
        {
            if (Position >= Guids.Length) return null;
            DragonNSPart res = Entries[Guids[Position]];
            Position++;
            return res;
        }

        private void ReadCorrection(string line)
        {
            //var regex = new Regex(@" GUID={(.*)} ");
            Match match = Regex.Match(line, @" GUID={(.*)} ");
            string guid = match.Value.Substring(match.Value.IndexOf('=')+1).Trim();
            if (!Entries.ContainsKey(guid)) return;
            DragonNSPart entry = Entries[guid];
            match = Regex.Match(line, @" corrPhrase='(.*)' ");
            string corr = match.Value.Substring(match.Value.IndexOf('=') + 1).Trim(new char[]{' ', '\''});
            entry.Text = corr;
        }

        private void ReadEntry(string line)
        {
            DragonNSPart entry = new DragonNSPart();
            line = line.Trim();
            int s1i = line.IndexOf('\t');
            string sId = line.Substring(0, s1i).Trim();
            entry.ID = Int32.Parse(sId);
            s1i = line.IndexOf('\t', s1i + 2);
            int s2i = line.IndexOf('\t', s1i + 2);
            string sStartTime = line.Substring(s1i, s2i - s1i).Trim();
            entry.TotalStartTime = ulong.Parse(sStartTime);
            int s3i = line.IndexOf('\t', s2i + 2);
            string sEndTime = line.Substring(s2i, s3i - s2i).Trim();
            entry.TotalEndTime = ulong.Parse(sEndTime);
            int s4i = line.IndexOf('\t', s3i + 2);
            entry.Guid = line.Substring(s3i, s4i - s3i).Trim();
            int cbi = line.IndexOf(')', s4i + 2);
            entry.Text = line.Substring(cbi+1).Trim();
            Entries.Add(entry.Guid, entry);
            Guids[Position] = entry.Guid;

            string wvId = entry.ID.ToString("D4");
            string wvPath = OutPath + "/wavs_" + FileBase + "_dat/" + "utt" + wvId + ".nwv";
            if (File.Exists(wvPath))
            {
                CopyWv(wvId, wvPath);
                entry.WavePath = WaveDir + "/utt" + wvId + ".wav";
            }
            Position++;
        }

        private void ReadEntryOld(string line)
        {
            DragonNSPart entry = new DragonNSPart();
            line = line.Trim();
            int s1i = line.IndexOf(' ');
            string sId = line.Substring(0, s1i).Trim();
            entry.ID = Int32.Parse(sId.Substring(0, sId.Length - 1));
            int s2i = line.IndexOf(' ', s1i + 2);
            string sStartTime = line.Substring(s1i, s2i - s1i).Trim();
            entry.TotalStartTime = ulong.Parse(sStartTime);
            int s3i = line.IndexOf(' ', s2i + 2);
            string sEndTime = line.Substring(s2i, s3i - s2i).Trim();
            entry.TotalEndTime = ulong.Parse(sEndTime);
            int s4i = line.IndexOf(' ', s3i + 2);
            entry.Guid = line.Substring(s3i, s4i - s3i).Trim();
            int cbi = line.IndexOf(')', s4i + 2);
            entry.Text = line.Substring(cbi + 1).Trim();
            Entries.Add(entry.Guid, entry);
            Guids[Position] = entry.Guid;

            string wvId = entry.ID.ToString("D4");
            string wvPath = OutPath + "/wavs_" + FileBase + "_dat/" + "utt" + wvId + ".nwv";
            CopyWv(wvId, wvPath);
            entry.WavePath = WaveDir + "/utt" + wvId + ".wav";
            Position++;
        }

        private void CopyWv(string wvId, string wvPath)
        {
            String args = String.Format("-f rif \"{0}\" \"{1}\"", wvPath, WaveDir + "/utt" + wvId + ".wav");
            Process p = NewProcess(Path.Combine(DNSExporterFolder,"sph2pipe.exe"), args);
            p.Start();
            p.WaitForExit();
        }

        private Process NewProcess(string filename, string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filename;
            startInfo.Arguments = args;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.EnvironmentVariables["USE_EXTERNAL_DFILE"] = "1";
            startInfo.EnvironmentVariables["PATH"] = Environment.GetEnvironmentVariable("PATH") + ";" + DNSExporterFolder;
            Process p = new Process();
            p.StartInfo = startInfo;
            return p;
        }

    }
}
