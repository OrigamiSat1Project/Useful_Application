using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace makePictureFromFMDownlink
{
    class Program
    {
        static void Main(string[] args)
        {
            char c = Path.DirectorySeparatorChar;
            string separater_header = "JQ1YCZ>JQ1ZHX:";
            string separater_footer = "\r\n";
            int actual_packet_length = 32;
            int including_other_packet_length = separater_header.Length + actual_packet_length + separater_footer.Length;
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
            FileStream fs = new FileStream(downlink_data_file_path, FileMode.Open);

            byte[] raw_data = new byte[fs.Length + (including_other_packet_length - fs.Length % including_other_packet_length)];
            fs.Read(raw_data, 0, raw_data.Length);
            fs.Close();
            Show("Now finish reading then close file...");
            string edited_data = string.Empty;
            //var packet_list = raw_data.Split(new string[] { separater }, StringSplitOptions.None);
            List<byte[]> packet_list = new List<byte[]>();
            int count = 0;
            byte[] pk_previous = new byte[actual_packet_length];
            int duplicate_count = 0;
            for (count = 0; count < (int)(raw_data.Length / including_other_packet_length); count++)
            {
                bool duplicate = true;
                int pk_index = including_other_packet_length * count;
                byte[] including_other_pk = new byte[including_other_packet_length];
                Buffer.BlockCopy(raw_data, pk_index, including_other_pk, 0, including_other_packet_length);
                byte[] pk = new byte[actual_packet_length];
                Buffer.BlockCopy(including_other_pk, separater_header.Length, pk, 0, actual_packet_length);

                for (int i = 0; i < actual_packet_length; i++)
                {
                    if (pk[i] != pk_previous[i])
                    {
                        duplicate = false;
                    }
                }
                if (!duplicate)
                {
                    packet_list.Add(pk);
                    Buffer.BlockCopy(pk, 0, pk_previous, 0, actual_packet_length);
                }
                else
                {
                    duplicate_count++;
                }
            }

            int actual_count = count - duplicate_count;
            byte[] actual_packets = new byte[actual_count * actual_packet_length];
            for (int i = 0; i < actual_count; i++)
            {
                Buffer.BlockCopy(packet_list[i], 0, actual_packets, actual_packet_length*i, actual_packet_length);
            }
            Show(actual_count + " / " + count + " packets is added. " + duplicate_count + " / " + count + " packets is duplicate...");
            Show("Now file open then write...");

            string edited_file_path = Path.GetDirectoryName(downlink_data_file_path) + c + Path.GetFileNameWithoutExtension(downlink_data_file_path) + "_edited_" + DateTime.Now.ToString("yyyyMMddhhss") + Path.GetExtension(downlink_data_file_path);
            if (File.Exists(edited_file_path))
            {
                File.Delete(edited_file_path);
            }
            FileStream _fs = new System.IO.FileStream(edited_file_path, FileMode.CreateNew, FileAccess.Write);
            _fs.Write(actual_packets, 0, actual_packets.Length);
            _fs.Close();
            Show("Now complete, new file path is [ " + edited_file_path + " ].");
        }

        static public void Show(string message)
        {
            Console.WriteLine(Path.GetFileName(Assembly.GetExecutingAssembly().Location) + "\t" + message);
        }
    }

}
