using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UI.Controls
{
    internal class ElasticKnobController : IKnobController
    {
        public event Action ValueChanged;
        public event Action StateChanged;

        private const double TimeInterval = 350;

        private double _moveValue;
        private double _moveStartValue;
        private double _time;
        private double _startPosition;
        private bool _freezePosition;
        private double _value;
        private bool _state;
        private bool _isEditable;
        private bool _captured;
        
        private FrameworkElement _knob;
        private FrameworkElement _host;
        
        public bool State
        {
            get
            {
                return _state;
            }
            set
            {
                var prevState = _state;

                _state = value;

                StateChanged?.Invoke();

                if (_state == true)
                {
                    if (Value <= 0.5)
                        MoveTo(1);
                }
                else
                {
                    if (Value > 0.5)
                        MoveTo(0);
                }
            }
        }
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;

                ValueChanged?.Invoke();

                if (_captured)
                {
                    if (_value > 0.5)
                        State = true;
                    else
                        State = false;
                }
            }
        }
        public bool IsEditable
        {
            get
            {
                return _isEditable;
            }
            set
            {
                _isEditable = value;
            }
        }

        public ElasticKnobController(FrameworkElement knob, FrameworkElement host)
        {
            _knob = knob;
            _host = host;
        }

        public void Move(double value)
        {
            if (_moveStartValue == -1)
            {
                _moveStartValue = _moveValue;
            }

            _moveValue = value;

            Update();
        }

        public void Start()
        {
            _time = DateTime.Now.TimeOfDay.TotalMilliseconds;
            _freezePosition = true;
            _moveStartValue = -1;
            _moveValue = -1;
            _startPosition = _knob.Margin.Left;
            _captured = true;
        }

        public void Stop()
        {
            _time = DateTime.Now.TimeOfDay.TotalMilliseconds;
            _freezePosition = false;
            _moveStartValue = -1;
            _moveValue = -1;
            _startPosition = 0;
            _captured = false;
        }

        private void ResetPosition()
        {
            var position = _knob.Margin.Left;
            var width = _knob.ActualWidth;
            var hostLength = _host.ActualWidth - width;

            if (Value > 0.5)
            {
                _startPosition = hostLength;
                _moveValue = 0;
            }
            else
            {
                _startPosition = 0;
                _moveValue = 0;
            }
        }

        private void MoveTo(double val)
        {
            var width = _knob.ActualWidth;
            var hostLength = _host.ActualWidth - width;

            _moveStartValue = -((val) * hostLength - 1 - hostLength);
            _moveValue = -((1 - val) * hostLength - 1 - hostLength);
            _startPosition = _knob.Margin.Left;
            _freezePosition = false;
            _captured = false;
        }

        public void Update()
        {
            if (_freezePosition)
            {
                var timeLeft = DateTime.Now.TimeOfDay.TotalMilliseconds - _time;

                if (timeLeft > TimeInterval && _isEditable)
                {
                    _freezePosition = false;
                }
            }

            var position = _knob.Margin.Left;
            var width = _knob.ActualWidth;
            var hostLength = _host.ActualWidth - width;

            if (_moveStartValue == -1 && _moveValue != -1)
                _moveStartValue = _moveValue;
            
            var currentOffset = _moveValue - _moveStartValue;
            var positionDiff = position - _startPosition;
            var movementDiff = currentOffset - positionDiff;

            if (!_captured)
                movementDiff = (State == true ? 1 : -1) * 150d;

            if (_captured)
            {
                if (_freezePosition)
                {
                    if (position < movementDiff)
                    {
                        width = _knob.ActualHeight + currentOffset;

                        hostLength = _host.ActualWidth - width;
                    }
                    else
                    {
                        width = _knob.ActualHeight + Math.Abs(currentOffset);

                        hostLength = _host.ActualWidth - width;

                        //if(State)
                        //    position = hostLength;
                        //else
                        //    position += movementDiff / 9;
                    }

                    if (width > _host.ActualWidth / 2 && _isEditable)
                    {
                        _freezePosition = false;
                    }
                    else if (width > _host.ActualWidth / 2 && !_isEditable)
                    {
                        width = _host.ActualWidth / 2;

                        hostLength = _host.ActualWidth - width;
                    }
                }
                else
                {
                    position += movementDiff / 9;

                    width -= _knob.ActualHeight / 27;

                    hostLength = _host.ActualWidth - width;
                }
            }
            else
            {
                position += movementDiff / 9;

                width -= _knob.ActualHeight / 27;
            }

            position = Math.Max(0, Math.Min(position, hostLength));
            _knob.Margin = new Thickness(position, _knob.Margin.Top,
                _knob.Margin.Right, _knob.Margin.Bottom);

            width = Math.Max(_knob.ActualHeight, Math.Min(width, _host.ActualWidth - position));
            _knob.Width = width;

            Value = 1 - (hostLength - position) / hostLength;

            //if ((Value > 0.999 || Value < 0.001) && !_captured && !_freezePosition)
            //{
            //    _captured = false;
            //    _freezePosition = false;
            //}
        }
    }
}
