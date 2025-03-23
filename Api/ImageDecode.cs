using ClickMashine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickMashine.Api
{
    public class ImageDecode
    {
        public required int ImageCategory { get; set; }  // Дополнительный строковый параметр
        public required IEnumerable<Bitmap> Files { get; set; } // Список загружаемых файлов
        public required EnumTypeSite Site { get; set; } // Тип сайта
    }
}
