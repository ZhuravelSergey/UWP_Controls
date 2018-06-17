using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media;
using UI.Controls.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Hosting;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace UI.Controls
{
    public sealed partial class Switch : UserControl
    {
        private EventRegistrationTokenTable<EventHandler<StateChangedEventArgs>> _stateChangedTokenTable = null;
        private EventRegistrationTokenTable<EventHandler<ValueChangedEventArgs>> _valueChangedTokenTable = null;

        public event EventHandler<StateChangedEventArgs> StateChanged
        {
            add
            {
                EventRegistrationTokenTable<EventHandler<StateChangedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable(ref _stateChangedTokenTable)
                    .AddEventHandler(value);
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<StateChangedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable(ref _stateChangedTokenTable)
                    .RemoveEventHandler(value);
            }
        }
        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add
            {
                EventRegistrationTokenTable<EventHandler<ValueChangedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable(ref _valueChangedTokenTable)
                    .AddEventHandler(value);
            }
            remove
            {
                EventRegistrationTokenTable<EventHandler<ValueChangedEventArgs>>
                    .GetOrCreateEventRegistrationTokenTable(ref _valueChangedTokenTable)
                    .RemoveEventHandler(value);
            }
        }

        public static DependencyProperty CornerRoundingProperty { get; private set; }
        public static DependencyProperty CornerRadiusProperty { get; private set; }
        public static DependencyProperty KnobCornerRadiusProperty { get; private set; }
        public static DependencyProperty KnobPaddingProperty { get; private set; }

        public static DependencyProperty BackgroundBrushProperty { get; private set; }
        public static DependencyProperty BackgroundContentProperty { get; private set; }
        public static DependencyProperty KnobBackgroundColorProperty { get; private set; }
        public static DependencyProperty KnobContentProperty { get; private set; }
        public static DependencyProperty BackgroundReflectionColorProperty { get; private set; }
        public static DependencyProperty KnobBorderOpacityProperty { get; private set; }

        public static DependencyProperty TextProperty { get; private set; }
        public new static DependencyProperty FontSizeProperty { get; private set; }
        public static DependencyProperty IsEditableProperty { get; private set; }
        public static DependencyProperty ValueProperty { get; private set; }
        public static DependencyProperty StateProperty { get; private set; }

        private IKnobController _knobController;

        public CornerRoundingType CornerRounding
        {
            get => (CornerRoundingType)GetValue(CornerRoundingProperty);
            set => SetValue(CornerRoundingProperty, value);
        }
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public CornerRadius KnobCornerRadius
        {
            get => (CornerRadius)GetValue(KnobCornerRadiusProperty);
            set => SetValue(KnobCornerRadiusProperty, value);
        }
        public double KnobPadding
        {
            get => (double)GetValue(KnobPaddingProperty);
            set => SetValue(KnobPaddingProperty, value);
        }
        public double KnobPaddingValue
        {
            get => Container.Height * (KnobPadding);
        }
        public double KnobMinWidth
        {
            get => Container.Height - KnobPaddingValue;
        }
        public double KnobMaxWidth
        {
            get => Container.ActualWidth - KnobPaddingValue * 2;
        }

        public Brush BackgroundBrush
        {
            get => GetValue(BackgroundBrushProperty) as Brush;
            set => SetValue(BackgroundBrushProperty, value);
        }
        public FrameworkElement BackgroundContent
        {
            get => GetValue(BackgroundContentProperty) as FrameworkElement;
            set => SetValue(BackgroundContentProperty, value);
        }
        public Color KnobBackgroundColor
        {
            get => (Color)GetValue(KnobBackgroundColorProperty);
            set => SetValue(KnobBackgroundColorProperty, value);
        }
        public FrameworkElement KnobContent
        {
            get => GetValue(KnobContentProperty) as FrameworkElement;
            set => SetValue(KnobContentProperty, value);
        }
        public Color BackgroundReflectionBrush
        {
            get => (Color)GetValue(BackgroundReflectionColorProperty);
            set => SetValue(BackgroundReflectionColorProperty, value);
        }
        public double KnobBorderOpacity
        {
            get => (double)GetValue(KnobBorderOpacityProperty);
            set => SetValue(KnobBorderOpacityProperty, value);
        }

        public string Text
        {
            get => GetValue(TextProperty) as string;
            set => SetValue(TextProperty, value);
        }
        public new int FontSize
        {
            get => (int)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public bool IsEditable
        {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }
        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public bool State
        {
            get => (bool)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        public IKnobController KnobController
        {
            get
            {
                return _knobController;
            }
            set
            {
                if (value == _knobController)
                    return;

                if(_knobController != null)
                {
                    _knobController.StateChanged -= _knobController_StateChanged;
                    _knobController.ValueChanged -= _knobController_ValueChanged;
                }

                _knobController = value;

                _knobController.StateChanged += _knobController_StateChanged;
                _knobController.ValueChanged += _knobController_ValueChanged;
            }
        }

        static Switch()
        {
            CornerRoundingProperty = DependencyProperty.Register("CornerRounding", typeof(CornerRoundingType),
                                        typeof(Switch), new PropertyMetadata(CornerRoundingType.Dynamic, CornerRoundingChanged));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius),
                                        typeof(Switch), new PropertyMetadata(new CornerRadius(1d), CornerRadiusChanged));

            KnobCornerRadiusProperty = DependencyProperty.Register("KnobCornerRadius", typeof(CornerRadius),
                                        typeof(Switch), new PropertyMetadata(new CornerRadius(1d), KnobCornerRadiusChanged));

            KnobPaddingProperty = DependencyProperty.Register("KnobPadding", typeof(double),
                                        typeof(Switch), new PropertyMetadata(0.05d, KnobPaddingChanged));


            BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush),
                                        typeof(Switch), new PropertyMetadata(new SolidColorBrush(Helpers.ColorHelper.GetColorFromHex("#FFFF6500")), 
                                        BackgroundBrushChanged));

            BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(FrameworkElement),
                                        typeof(Switch), new PropertyMetadata(null, BackgroundContentChanged));

            KnobBackgroundColorProperty = DependencyProperty.Register("KnobBackgroundBrush", typeof(Color),
                                        typeof(Switch), new PropertyMetadata(Helpers.ColorHelper.GetColorFromHex("#FFE5D2D2"), 
                                        KnobBackgroundBrushChanged));

            KnobContentProperty = DependencyProperty.Register("KnobContent", typeof(FrameworkElement),
                                        typeof(Switch), new PropertyMetadata(0.05d, KnobContentChanged));

            BackgroundReflectionColorProperty = DependencyProperty.Register("BackgroundReflectionBrush", typeof(Color),
                                        typeof(Switch), new PropertyMetadata(0.05d, BackgroundReflectionBrushChanged));

            KnobBorderOpacityProperty = DependencyProperty.Register("KnobBorderOpacity", typeof(double), typeof(Switch),
                                        new PropertyMetadata(0.025d, KnobBorderOpacityChanged));


            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(Switch), new PropertyMetadata("", TextChanged));

            FontSizeProperty = DependencyProperty.Register("FontSize", typeof(int), typeof(Switch), new PropertyMetadata(12, FontSizeChanged));

            IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(Switch),
                                        new PropertyMetadata(true, IsEditableChanged));

            ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(Switch), null);

            StateProperty = DependencyProperty.Register("State", typeof(bool), typeof(Switch),
                                        new PropertyMetadata(false, OnStateChanged));
        }

        public Switch()
        {
            this.InitializeComponent();

            this.SizeChanged += Switch_SizeChanged;
            this.Loaded += Switch_Loaded;

            KnobController = new ElasticKnobController(Knob, Container);

            BackgroundRectReflectionAnimation.Begin();
            BackgroundRectReflectionAnimation.Completed += BackgroundRectReflectionAnimation_Completed;

            Windows.UI.Xaml.Media.CompositionTarget.Rendering += CompositionTarget_Rendering;

            SetBlurEffect(KnobBorder);

            Text = "";
            FontSize = 12;
            IsEditable = true;
        }

        private void SetBlurEffect(UIElement glassHost)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;

            var blurEffect = new GaussianBlurEffect
            {
                BlurAmount = 5.0f,
                BorderMode = EffectBorderMode.Hard,
                Source = new ArithmeticCompositeEffect
                {
                    MultiplyAmount = 0,
                    Source1Amount = 1f,
                    Source2Amount = 0.0f,
                    Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                    Source2 = new ColorSourceEffect
                    {
                        Color = Colors.White
                    }
                }
            };

            var effectFactory = compositor.CreateEffectFactory(blurEffect);
            var backdropBrush = compositor.CreateBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();

            effectBrush.SetSourceParameter("backdropBrush", backdropBrush);

            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = effectBrush;

            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);

            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

        private void UpdateStructure()
        {
            Container.Height = Container.ActualWidth / 3;

            var containerHeight = Container.Height;
            
            Knob.Height = containerHeight;

            TextContainer.Margin = new Thickness(containerHeight, 0, containerHeight, 0);

            var knobSize = containerHeight - KnobPaddingValue;
            var containerSH = containerHeight / 2;
            var knobSH = KnobButton.ActualHeight / 2;

            KnobButton.Margin = new Thickness(KnobPaddingValue);
            KnobShadow.Margin = new Thickness(KnobPaddingValue, KnobPaddingValue * 2, KnobPaddingValue, 0);

            #region CORNERS ROUNDING
            if (CornerRounding == CornerRoundingType.Dynamic)
            {
                Container.CornerRadius = new CornerRadius(containerSH * (CornerRadius.TopLeft),
                                                            containerSH * (CornerRadius.TopRight),
                                                            containerSH * (CornerRadius.BottomRight),
                                                            containerSH * (CornerRadius.BottomLeft));

                KnobButton.CornerRadius = new CornerRadius(knobSH * (KnobCornerRadius.TopLeft),
                                                            knobSH * (KnobCornerRadius.TopRight),
                                                            knobSH * (KnobCornerRadius.BottomRight),
                                                            knobSH * (KnobCornerRadius.BottomLeft));

                KnobShadow.CornerRadius = new CornerRadius(knobSH * (KnobCornerRadius.TopLeft),
                                                            knobSH * (KnobCornerRadius.TopRight),
                                                            knobSH * (KnobCornerRadius.BottomRight),
                                                            knobSH * (KnobCornerRadius.BottomLeft));

                KnobBorder.CornerRadius = Container.CornerRadius;
            }
            else if (CornerRounding == CornerRoundingType.Static)
            {
                Container.CornerRadius = new CornerRadius((CornerRadius.TopLeft),
                                                            (CornerRadius.TopRight),
                                                            (CornerRadius.BottomRight),
                                                            (CornerRadius.BottomLeft));

                Knob.CornerRadius = new CornerRadius((KnobCornerRadius.TopLeft),
                                                            (KnobCornerRadius.TopRight),
                                                            (KnobCornerRadius.BottomRight),
                                                            (KnobCornerRadius.BottomLeft));

                KnobShadow.CornerRadius = new CornerRadius((KnobCornerRadius.TopLeft),
                                                            (KnobCornerRadius.TopRight),
                                                            (KnobCornerRadius.BottomRight),
                                                            (KnobCornerRadius.BottomLeft));

                KnobBorder.CornerRadius = Container.CornerRadius;
            }
            #endregion
        }

        protected void OnStateChanged(bool state)
        {
            EventRegistrationTokenTable<EventHandler<StateChangedEventArgs>>
            .GetOrCreateEventRegistrationTokenTable(ref _stateChangedTokenTable)
            .InvocationList?.Invoke(this, new StateChangedEventArgs(state));
        }

        protected void OnValueChanged(double value)
        {
            EventRegistrationTokenTable<EventHandler<ValueChangedEventArgs>>
            .GetOrCreateEventRegistrationTokenTable(ref _valueChangedTokenTable)
            .InvocationList?.Invoke(this, new ValueChangedEventArgs(value));
        }

        #region DEPENDENCY_PROPERTY_CHANGED  HANDLERS
        private static void KnobPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Switch).UpdateStructure();
        }

        private static void KnobCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Switch).UpdateStructure();
        }

        private static void CornerRoundingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Switch).UpdateStructure();
        }

        private static void CornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Switch).UpdateStructure();
        }
        

        private static void BackgroundReflectionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;
        }

        private static void KnobContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;
        }

        private static void KnobBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;
        }

        private static void BackgroundContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;
        }

        private static void BackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;

            sw.BackgroundRect.Fill = sw.BackgroundBrush;
        }

        private static void KnobBorderOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;

            sw.KnobBorder.Background = new SolidColorBrush(Color.FromArgb(
                Convert.ToByte(Math.Min(255, Math.Max(0, sw.KnobBorderOpacity * 255))), 0, 0, 0));
        }


        private static void TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;

            sw.TextContainer.Text = sw.Text;
        }

        private static void FontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;

            sw.TextContainer.FontSize = sw.FontSize;
        }

        private static void IsEditableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;

            sw.KnobController.IsEditable = sw.IsEditable;
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sw = d as Switch;

            sw.KnobController.State = sw.State;
        }
        #endregion

        #region KNOB EVENT HANDLERS
        private void KnobButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(KnobEnterAnimation.GetCurrentState() == Windows.UI.Xaml.Media.Animation.ClockState.Active))
            {
                //var pointerPosition = e.GetCurrentPoint(KnobBackgroundRect);

                //var y_start = pointerPosition.Position.Y / KnobBackgroundRect.ActualHeight;
                //var x_start = pointerPosition.Position.X / KnobBackgroundRect.ActualWidth;

                //var y_end = 1 - y_start;
                //var x_end = 1 - x_start;

                //Debug.WriteLine($"start: {x_start}  {y_start}");
                //Debug.WriteLine($"end: {x_end}  {y_end}");

                //KnobBackgroundRectBrush.EndPoint = new Point(x_end, y_end);
                //KnobBackgroundRectBrush.StartPoint = new Point(x_start, y_start);

                KnobExitAnimation.Stop();
                KnobEnterAnimation.Begin();
            }
        }

        private void KnobButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (!(KnobExitAnimation.GetCurrentState() == Windows.UI.Xaml.Media.Animation.ClockState.Active))
            {
                KnobEnterAnimation.Stop();
                KnobExitAnimation.Begin();
            }
        }

        private void KnobButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            KnobButton.CapturePointer(e.Pointer);

            KnobController.Start();
        }

        private void KnobButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            KnobButton.ReleasePointerCapture(e.Pointer);

            KnobController.Stop();
        }

        private void KnobButton_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(Container);

            if (!(point.Properties.IsLeftButtonPressed))
                return;

            KnobController.Move(point.Position.X);

            //UpdateStructure();
        }

        private void _knobController_ValueChanged()
        {
            Value = KnobController.Value;

            OnValueChanged(Value);
        }

        private void _knobController_StateChanged()
        {
            State = KnobController.State;

            OnStateChanged(State);
        }
        #endregion

        #region OTHER EVENT HANDLERS
        private void Switch_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateStructure();
        }

        private void Container_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (!(BackgroundRectReflectionAnimation.GetCurrentState() == Windows.UI.Xaml.Media.Animation.ClockState.Active))
                BackgroundRectReflectionAnimation.Begin();

            UpdateStructure();
        }

        private void Switch_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateStructure();
        }

        private void CompositionTarget_Rendering(object sender, object e)
        {
            KnobController.Update();
            UpdateStructure();
        }

        private async void BackgroundRectReflectionAnimation_Completed(object sender, object e)
        {
            if (!(BackgroundRectReflectionAnimation.GetCurrentState() == Windows.UI.Xaml.Media.Animation.ClockState.Active))
            {
                await Task.Delay(5000);

                BackgroundRectReflectionAnimation.Begin();
            }
        }
        #endregion
    }
}
