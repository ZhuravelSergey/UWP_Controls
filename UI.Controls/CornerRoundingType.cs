using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Controls
{
    /// <summary>
    /// Определяет способ округления углов
    /// </summary>
    public enum CornerRoundingType
    {
        /// <summary>
        /// Округление зависит от размеров элемента. Значения в диапазоне 0 - 1, где 0 - без округления,
        /// 1 - округление в половину высоты элемента (максимально возможное)
        /// </summary>
        Dynamic,
        /// <summary>
        /// Статичное округление, указывается в стандартных единицах измерения
        /// </summary>
        Static
    }
}
