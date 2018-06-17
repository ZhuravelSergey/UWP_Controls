using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Viewer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private double _padding;
        private bool _isEditable;
        private double _knobBorderOpacity;
        private string _text;
        private int _fontSize;
        private double _cornerRadius;

        public double Padding
        {
            get
            {
                return _padding;
            }
            set
            {
                _padding = value;

                RaisePropertyChanged();
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

                RaisePropertyChanged();
            }
        }
        public double KnobBorderOpacity
        {
            get
            {
                return _knobBorderOpacity;
            }
            set
            {
                _knobBorderOpacity = value;

                RaisePropertyChanged();
            }
        }
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                RaisePropertyChanged();
            }
        }
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;

                RaisePropertyChanged();
            }
        }
        public double CornerRadius
        {
            get
            {
                return _cornerRadius;
            }
            set
            {
                _cornerRadius = value;

                RaisePropertyChanged();
            }
        }

        public MainViewModel()
        {
            Padding = 0.05;
            IsEditable = false;
            KnobBorderOpacity = 0.05;
            Text = "it is text";
            FontSize = 12;
            CornerRadius = 1d;
        }
    }
}
