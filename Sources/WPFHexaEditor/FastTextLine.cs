//////////////////////////////////////////////
// Apache 2.0  - 2016-2020
// Author : Derek Tremblay (derektremblay666@gmail.com)
//////////////////////////////////////////////

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace WpfHexaEditor
{
    /// <summary>
    /// Light Label like control
    /// </summary>
    internal class FastTextLine : FrameworkElement
    {
        private readonly HexEditor _parent;

        #region Constructor

        public FastTextLine(HexEditor parent)
        {
            //Parent hexeditor
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            //Default properties
            DataContext = this;
        }

        #endregion Contructor

        #region Base properties

        /// <summary>
        /// Definie the foreground
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty =
            TextElement.ForegroundProperty.AddOwner(typeof(FastTextLine));

        public Brush Foreground
        {
            get => (Brush)GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        public static readonly DependencyProperty BackgroundProperty =
            TextElement.BackgroundProperty.AddOwner(typeof(FastTextLine),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Defines the background
        /// </summary>
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FastTextLine),
                //Only AffectsRender: width is set explicitly in OnRender when AutoWidth is on,
                //so a text change repaints without forcing a measure pass per scrolled line.
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Text to be displayed representation of Byte
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty FontWeightProperty =
            TextElement.FontWeightProperty.AddOwner(typeof(FastTextLine));

        /// <summary>
        /// The FontWeight property specifies the weight of the font.
        /// </summary>
        public FontWeight FontWeight
        {
            get => (FontWeight)GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        #endregion Base properties

        #region Properties

        /// <summary>
        /// Get or set if the width are auto or fixed
        /// </summary>
        public bool AutoWidth { get; set; } = true;

        /// <summary>
        /// Get or set the render point
        /// </summary>
        public Point RenderPoint
        {
            get => (Point)GetValue(RenderPointProperty);
            set => SetValue(RenderPointProperty, value);
        }

        public static readonly DependencyProperty RenderPointProperty =
            DependencyProperty.Register(nameof(RenderPoint), typeof(Point), typeof(FastTextLine),
                new FrameworkPropertyMetadata(new Point(0, 0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        /// Set the highlight (weight + foreground) and repaint only if something actually changed.
        /// Used for cheap per-keystroke header column highlighting.
        /// </summary>
        internal void SetHighLight(FontWeight weight, Brush foreground)
        {
            if (FontWeight == weight && ReferenceEquals(Foreground, foreground))
                return;

            FontWeight = weight;
            Foreground = foreground;
            InvalidateVisual();
        }

        #endregion

        /// <summary>
        /// Render the control
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            //Draw background
            if (Background is not null)
                dc.DrawRectangle(Background, null, new Rect(0, 0, RenderSize.Width, RenderSize.Height));

            //Draw text (typeface and pixelsPerDip are cached on the parent to avoid per-render allocation)
            var formatedText = new FormattedText(Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                _parent.GetTypeface(FontWeight), _parent.FontSize, Foreground, _parent.PixelsPerDip);

            dc.DrawText(formatedText, new Point(RenderPoint.X, RenderPoint.Y));

            if (AutoWidth)
                Width = formatedText.Width + RenderPoint.X;
        }
    }
}
