using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shrader.IDE.Model
{
    public class SettingModel
    {
        public bool IsTime { get; set; }
        public bool IsMouse { get; set; }
        public bool IsViewPort { get; set; }
        public IEnumerable<string> ImagesPath { get; set; }
    }
}
