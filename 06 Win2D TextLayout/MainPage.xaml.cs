// Win2D TextLayout
// Get glyph info from string in a UWP app
// Original code extracted from Win2D samples gallery, simplified here
// 2020-12  PV

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace Win2DTextLayout
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        void Canvas_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            EnsureResources(sender, sender.Size);

            currentDrawingSession = args.DrawingSession;

            textLayout.SetBrush(0, testString.Length, null);

            hasSelection = true;
            selectionStartIndex = 1;
            selectionEndIndex = 4;
            if (hasSelection)
            {
                int firstIndex = Math.Min(selectionStartIndex, selectionEndIndex);
                int length = Math.Abs(selectionEndIndex - selectionStartIndex) + 1;
                CanvasTextLayoutRegion[] descriptions = textLayout.GetCharacterRegions(firstIndex, length);
                foreach (CanvasTextLayoutRegion description in descriptions)
                {
                    args.DrawingSession.FillRectangle(InflateRect(description.LayoutBounds), Colors.White);
                }
                textLayout.SetBrush(firstIndex, length, selectionTextBrush);
            }

            textLayout.SetInlineObject(0, testString.Length, null);

            textLayout.Direction = CanvasTextDirection.LeftToRightThenTopToBottom;      // Default anyway

            args.DrawingSession.DrawTextLayout(textLayout, 0, 0, textBrush);

            if (true || ShowPerCharacterLayoutBounds)
            {
                for (int i = 0; i < testString.Length; i++)
                {
                    textLayout.GetCaretPosition(i, false, out CanvasTextLayoutRegion textLayoutRegion);
                    args.DrawingSession.DrawRectangle(textLayoutRegion.LayoutBounds, Colors.Blue, 2);
                }
            }
        }

        Rect InflateRect(Rect r)
        {
            return new Rect(
                new Point(Math.Floor(r.Left), Math.Floor(r.Top)),
                new Point(Math.Ceiling(r.Right), Math.Ceiling(r.Bottom)));
        }



        CanvasTextLayout textLayout;
        CanvasLinearGradientBrush textBrush;
        CanvasLinearGradientBrush selectionTextBrush;

        readonly SpecialGlyph inlineObject = new SpecialGlyph();

        string testString;

        public bool UseEllipsisTrimming { get; set; }

        public bool ApplyInlineObjects { get; set; }

        public bool ShowPerCharacterLayoutBounds { get; set; }
        public bool ShowLayoutBounds { get; set; }
        public bool ShowLayoutBoundsWithTrailingWhitespace { get; set; }
        public bool ShowDrawBounds { get; set; }

        bool needsResourceRecreation;
        Size resourceRealizationSize;

        bool hasSelection;
        int selectionStartIndex = 0;
        int selectionEndIndex = 0;

        //
        // This is stored as a local static so that SpecialGlyph has access to it.
        // This app only has one inline object type, but this pattern generalizes
        // well to apps with multiple inline objects which all need to know about
        // a drawing session.
        //
        static CanvasDrawingSession currentDrawingSession;

        class SpecialGlyph : ICanvasTextInlineObject
        {
            CanvasBitmap resourceBitmap;
            float size;

            public void Draw(ICanvasTextRenderer renderer, Vector2 position, bool isSideways, bool isRightToLeft, object brush)
            {
                currentDrawingSession.DrawImage(resourceBitmap, new Rect(position.X, position.Y, size, size));
            }

            public void SetBitmap(CanvasBitmap bitmap)
            {
                resourceBitmap = bitmap;
            }

            public void SetLayout(CanvasTextLayout layout)
            {
                size = layout.DefaultFontSize;
            }

            public float Baseline { get { return size; } }
            public Rect DrawBounds { get { return new Rect(0, 0, size, size); } }
            public Size Size { get { return new Size(size, size); } }
            public bool SupportsSideways { get { return false; } }
            public CanvasLineBreakCondition BreakBefore { get { return CanvasLineBreakCondition.CanBreak; } }
            public CanvasLineBreakCondition BreakAfter { get { return CanvasLineBreakCondition.CanBreak; } }
        }



        void EnsureResources(ICanvasResourceCreatorWithDpi resourceCreator, Size targetSize)
        {
            if (resourceRealizationSize == targetSize && !needsResourceRecreation)
                return;

            float canvasWidth = (float)targetSize.Width;
            float canvasHeight = (float)targetSize.Height;

            if (textLayout != null)
                textLayout.Dispose();
            textLayout = CreateTextLayout(resourceCreator, canvasWidth, canvasHeight);

            inlineObject.SetLayout(textLayout);

            Rect layoutBounds = textLayout.LayoutBounds;

            textBrush = new CanvasLinearGradientBrush(resourceCreator, Colors.Red, Colors.Green)
            {
                StartPoint = new System.Numerics.Vector2((float)(layoutBounds.Left + layoutBounds.Right) / 2, (float)layoutBounds.Top),
                EndPoint = new System.Numerics.Vector2((float)(layoutBounds.Left + layoutBounds.Right) / 2, (float)layoutBounds.Bottom)
            };

            selectionTextBrush = new CanvasLinearGradientBrush(resourceCreator, Colors.Green, Colors.Red)
            {
                StartPoint = textBrush.StartPoint,
                EndPoint = textBrush.EndPoint
            };

            needsResourceRecreation = false;
            resourceRealizationSize = targetSize;
        }


        private CanvasTextLayout CreateTextLayout(ICanvasResourceCreator resourceCreator, float canvasWidth, float canvasHeight)
        {
            float sizeDim = Math.Min(canvasWidth, canvasHeight);
            //testString = "Aé♫山𝄞🐗\r\nœæĳøß≤≠Ⅷﬁﬆ\r\n🐱‍🏍 🐱‍👓 🐱‍🚀 🐱‍👤 🐱‍🐉 🐱‍💻\r\n🧝 🧝‍♂️ 🧝‍♀️ 🧝🏽 🧝🏽‍♂️ 🧝🏽‍♀️";
            //testString = "Abc𝄞\r\nOù\r\n🐗🧔🏻";
            testString = "<🧝‍♀️>";
            CanvasTextFormat textFormat = new CanvasTextFormat()
            {
                FontSize = sizeDim * 0.1f,
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Top
            };

            textFormat.FontFamily = "Arial"; // fontPicker.CurrentFontFamily;

            textFormat.TrimmingGranularity = CanvasTextTrimmingGranularity.Word;
            textFormat.TrimmingSign = CanvasTrimmingSign.Ellipsis;  //  UseEllipsisTrimming ? CanvasTrimmingSign.Ellipsis : CanvasTrimmingSign.None;

            var textLayout = new CanvasTextLayout(resourceCreator, testString, textFormat, canvasWidth, canvasHeight);

            return textLayout;
        }
    }
}
