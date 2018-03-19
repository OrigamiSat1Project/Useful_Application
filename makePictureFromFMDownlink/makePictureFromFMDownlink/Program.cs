using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace makePictureFromFMDownlink
{
    class Program
    {
        static void Main(string[] args)
        {
            char c = Path.DirectorySeparatorChar;
            string separater = "JQ1YCZ>JQ1ZHX:";
            string footer = "\r\n";
            if (args.Length != 1)
            {
                Show("Usage : makePictureFromFMDownlink.exe [downlink_file_path]");
                Environment.Exit(1);
            }
            string downlink_data_file_path = args[0];
            if (!File.Exists(downlink_data_file_path))
            {
                Show("Such name file does not existed...");
                Environment.Exit(1);
            }

            Show("Now file open then read...");
            StreamReader sr = new StreamReader(downlink_data_file_path, Encoding.Default);
            string raw_data = sr.ReadToEnd();
            string edited_data = string.Empty;
            var packet_list = raw_data.Split(new string[] { separater }, StringSplitOptions.None);
            int raw_lines = packet_list.Length;

            int index = 0;
            int lines = 0;
            string pk_already_get = string.Empty;
            foreach (var pk in packet_list)
            {
                if (pk != pk_already_get)
                {
                    lines++;
                    int index_last = pk.Length - 1;
                    if (pk.Substring(index_last, 1) == "\n")
                    {
                        pk.Remove(index_last, 1);
                    }
                    index_last = pk.Length - 1;
                    if (pk.Substring(index_last, 1) == "\r")
                    {
                        pk.Remove(index_last, 1);
                    }
                    edited_data += pk;
                    Show(lines + " lines already added.");
                }
                pk_already_get = pk;
            }
            Show(lines + " lines is added. " + (raw_lines - lines) + " lines is duplicate...");
            Show("Now file open then write...");

            string edited_file_path = Path.GetDirectoryName(downlink_data_file_path) + c + Path.GetFileNameWithoutExtension(downlink_data_file_path) + "_edited_" + DateTime.Now.ToString("yyyyMMddhhss") + Path.GetExtension(downlink_data_file_path);
            if (File.Exists(edited_file_path))
            {
                File.Delete(edited_file_path);
            }
            StreamWriter sw = new StreamWriter(edited_file_path, false, Encoding.Default);
            sw.Write(edited_data);
            Show("Now complete, new file path is [" + edited_file_path + "].");
            Show("If you want to see as jpeg, please change extension to \".jpg\".");
        }

        static public void Show(string message)
        {
            Console.WriteLine(message);
        }
    }

}
