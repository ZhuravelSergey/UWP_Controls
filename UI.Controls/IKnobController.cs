using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Controls
{
    public interface IKnobController
    {
        event Action ValueChanged;
        event Action StateChanged;

        /// <summary>
        /// Текущее значение позиции элемента Knob в диапазон 0..1
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Указывает на текущее состояние элемента
        /// </summary>
        bool State { get; set; }

        /// <summary>
        /// Указывает на значение, определяющее может ли быть изменено текущее значение свойства State
        /// </summary>
        bool IsEditable { get; set; }

        /// <summary>
        /// Выполняет передвижение элемента
        /// </summary>
        /// <param name="value"></param>
        void Move(double value);

        /// <summary>
        /// Вызывается при "захвате" мышкой элемента Knob
        /// </summary>
        void Start();

        /// <summary>
        /// Вызывается, когда пользователь "отпускает" элемент Knob
        /// </summary>
        void Stop();

        /// <summary>
        /// Вызываеся при рендеринге каждого кадра
        /// </summary>
        void Update();
    }
}
