using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace MetronomeBeat {
    internal static class CsvOut {
        internal static void write(List<DateTime>[] lines) {
            string path = @".\output" + DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".csv";
            FileStream fs = File.Create(path);
            fs.Close();
            StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
            for(int i = 0; i < Math.Max(Math.Max(lines[0].Count, lines[1].Count), lines[2].Count); i++) {
                string first = lines[0].Count > i ? lines[0][i].ToString("ss.fff") : "9999";
                string second = lines[1].Count > i ? lines[1][i].ToString("ss.fff") : "9999";
                string third = lines[2].Count > i ? lines[2][i].ToString("ss.fff") : "9999";
                writer.WriteLine(string.Format("{0},{1},{2}", first, second, third));
                Debug.WriteLine(string.Format("{0},{1},{2}", first, second, third));
            }
            writer.Close();
        }
    }
}
