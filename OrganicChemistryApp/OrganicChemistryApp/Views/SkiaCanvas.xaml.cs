using System;
using System.Collections.Generic;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Forms;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace OrganicChemistryApp.Views
{
    public partial class SkiaCanvas : ContentPage
    { 
        
        bool showFill = true;
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        float curGradient;
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };


        public SkiaCanvas()
        {
            InitializeComponent();
        }


        /*void OnCanvasViewTapped(object sender, EventArgs args)
        {
            
            showFill ^= true;
            (sender as SKCanvasView).InvalidateSurface();
        }*/

        /*void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
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
        }*/

        void TouchEffect_OnTouchAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        var firstPoint = path[0];
                        path.Rewind();
                        path.MoveTo(firstPoint);
                        path.LineTo(ConvertToPixel(args.Location));
                        SKPathMeasure measure = new SKPathMeasure(path);
                        var pGradient = Gradient(path);
                        Title = $"M: {pGradient}";
                        if (Math.Abs(curGradient) > 0.25 && measure.Length > 120 
                                                         && Math.Abs(Math.Abs(pGradient) - Math.Abs(curGradient)) > 0.1)
                        {
                            if (completedPaths.Count == 0)
                                MakeNewPath(sender, args);

                            else
                            {
                                var lastGrad = Gradient(completedPaths.Last());
                                float meas = new SKPathMeasure(completedPaths.Last()).Length;
                                if (lastGrad * pGradient < 0)
                                {
                                    float percAngError = Math.Abs((Math.Abs(lastGrad) - Math.Abs(pGradient)) / lastGrad);
                                    float percLenError = Math.Abs((meas - measure.Length) / meas);
                                    Title += $"%{percAngError}";
                                    if (percAngError < 0.3 /* && percLenError < 0.3*/)
                                        MakeNewPath(sender, args);
                                }
                            }
                        }
                        curGradient = pGradient;
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;

            }
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();
            foreach (SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }
        }

        SKPoint ConvertToPixel(TouchTrackingPoint pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        float Gradient(SKPath path)
        {
            var line = path.GetLine();
            return -(line[1].Y - line[0].Y) / (line[1].X - line[0].X);
        }

        private void ClearCanvas_OnClicked(object sender, EventArgs e)
        {
            inProgressPaths = new Dictionary<long, SKPath>();
            completedPaths = new List<SKPath>();
            curGradient = 0;
            Title = "Draw";
            canvasView.InvalidateSurface();
        }

        private void MakeNewPath(object sender, TouchActionEventArgs args)
        {
            completedPaths.Add(inProgressPaths[args.Id]);
            inProgressPaths.Remove(args.Id);
            SKPath newPath = new SKPath();
            newPath.MoveTo(ConvertToPixel(args.Location));
            inProgressPaths.Add(args.Id, newPath);
            canvasView.InvalidateSurface();
        }
    }

}