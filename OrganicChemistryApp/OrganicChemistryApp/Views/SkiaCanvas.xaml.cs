using System;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OrganicChemistryApp.Views
{
    public partial class SkiaCanvas : ContentPage
    { 
        
        bool showFill = true;

        public SkiaCanvas()
        {
            InitializeComponent();
        }


        void OnCanvasViewTapped(object sender, EventArgs args)
        {
            
            showFill ^= true;
            (sender as SKCanvasView).InvalidateSurface();
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.DarkMagenta.ToSKColor(),
                StrokeWidth = 50
            };
            canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);

            if (showFill)
            {
                paint.Style = SKPaintStyle.Fill;
                paint.Color = SKColors.DarkOliveGreen;
                canvas.DrawCircle(info.Width / 2, info.Height / 2, 100, paint);
            }
        }
    }

}