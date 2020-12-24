using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFsoft.ExHttpClient
{
    public class ExCommon
    {
        public static string HumanReadableFilesize(long? _size)
        {
            var size = Convert.ToDouble(_size);
            string[] units = new string[] { "B", "KB", "MB", "GB", "TB", "PB" };
            double mod = 1024.0;
            int i = 0;
            while (size >= mod)
            {
                size /= mod;
                i++;
            }
            var unit = units[i];
            if (unit != "B" && unit != "KB")
                return Math.Round(size, 2) + " " + unit;
            else return Math.Round(size) + " " + unit;
        }
    }
}
