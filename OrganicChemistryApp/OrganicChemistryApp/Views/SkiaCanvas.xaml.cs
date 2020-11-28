using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Forms;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;

namespace OrganicChemistryApp.Views
{
    public partial class SkiaCanvas : ContentPage
    {

        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();
        Dictionary<SKPoint, List<SKPath>> guidePaths = new Dictionary<SKPoint, List<SKPath>>();

        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        private SKPaint guidePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.CornflowerBlue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            PathEffect = SKPathEffect.CreateDash(new float[] {15, 15}, 20)
        };


        public SkiaCanvas()
        {
            InitializeComponent();
        }

        
        void TouchEffect_OnTouchAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        guidePaths.Clear();
                        SKPath path = new SKPath();
                        var pixel = ConvertToPixel(args.Location);

                        if (completedPaths.Count > 0)
                        {
                            foreach (var pth in completedPaths)
                            {
                                if (SKPoint.Distance(pth[0], pixel) < 50)
                                {
                                    pixel = pth[0];
                                    break;
                                }
                                if (SKPoint.Distance(pth.LastPoint, pixel) < 50)
                                {
                                    pixel = pth.LastPoint;
                                    break;
                                }
                            }
                        }

                        path.MoveTo(pixel);
                        inProgressPaths.Add(args.Id, path);

                        
                        SuggestGuidePaths(pixel);

                        var gp = new List<SKPoint>();
                        var cp = new List<SKPoint>();
                        foreach (var p in guidePaths[pixel])
                            gp.Add(p.LastPoint);
                        foreach (var p in completedPaths)
                            cp.AddRange(p.GetLine());

                        var intersect = cp.Intersect(gp).ToList();

                        if (intersect.Count() > 1)
                        {
                            SuggestAltGuidePaths(pixel);
                        }

                        guidePaths[pixel].RemoveAll(pth => intersect.Contains(pth.LastPoint));

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

                        if (guidePaths.ContainsKey(firstPoint))
                        {
                            SKPath linqPath = guidePaths[firstPoint]
                                .FirstOrDefault(pth => SKPoint.Distance(pth.LastPoint, path.LastPoint) < 30);
                            if (linqPath != null)
                            {
                                guidePaths.Remove(linqPath[0]);
                                completedPaths.Add(linqPath);
                                MakeNewPathFromPoint(args, linqPath.LastPoint);
                                SuggestGuidePaths(linqPath.LastPoint);
                                canvasView.InvalidateSurface();
                            }
                        }
                        canvasView.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
                        //completedPaths.Add(inProgressPaths[args.Id]);
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
            foreach (SKPath path in guidePaths.Values.SelectMany(x => x))
            {
                canvas.DrawPath(path, guidePaint);
            }

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
            return new SKPoint((float) (canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float) (canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        float Gradient(SKPath path)
        {
            var line = path.GetLine();
            return -(line[1].Y - line[0].Y) / (line[1].X - line[0].X);
        }

        private void ClearCanvas_OnClicked(object sender, EventArgs e)
        {
            inProgressPaths.Clear();
            completedPaths.Clear();
            guidePaths.Clear();
            canvasView.InvalidateSurface();
        }

        private void MakeNewPath(TouchActionEventArgs args)
        {
            //completedPaths.Add(inProgressPaths[args.Id]);
            inProgressPaths.Remove(args.Id);
            SKPath newPath = new SKPath();
            newPath.MoveTo(ConvertToPixel(args.Location));
            inProgressPaths.Add(args.Id, newPath);
        }

        private void MakeNewPathFromPoint(TouchActionEventArgs args, SKPoint pt)
        {
            inProgressPaths.Remove(args.Id);
            SKPath newPath = new SKPath();
            newPath.MoveTo(pt);
            inProgressPaths.Add(args.Id, newPath);
        }

        private void MakeNewGuidePath(TouchActionEventArgs args, SKPath path)
        {
            guidePaths.Clear();
            var line = path.GetLine();
            var newPath = new SKPath();
            newPath.MoveTo(line[1]);
            SKPoint point = new SKPoint
            {
                X = 2 * line[1].X - line[0].X,
                Y = line[0].Y
            };
            path.LineTo(point);
            guidePaths.Add(ConvertToPixel(args.Location), new List<SKPath> {path});
        }

        private void SuggestGuidePaths(SKPoint pixel)
        {
            float x = pixel.X;
            float y = pixel.Y;
            var pathList = new List<SKPath>();
            const float len = 200;
            const double ang = 0.5477400137;

            var path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x + len * (float) Math.Cos(ang), y + len * (float) Math.Sin(ang));
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x + len * (float) Math.Cos(ang), y - len * (float) Math.Sin(ang));
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x - len * (float) Math.Cos(ang), y + len * (float) Math.Sin(ang));
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x - len * (float) Math.Cos(ang), y - len * (float) Math.Sin(ang));
            pathList.Add(path);

            guidePaths.Add(pixel, pathList);

        }

        private void SuggestAltGuidePaths(SKPoint pixel)
        {
            float x = pixel.X;
            float y = pixel.Y;
            var pathList = new List<SKPath>();
            const float len = 200;
            const float ratio = 0.7F;

            var path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x, y + len * ratio);
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x, y - len * ratio);
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x + len * ratio, y);
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x - len * ratio, y);
            pathList.Add(path);

            guidePaths[pixel].AddRange(pathList);
        }

        private void Undo_OnClicked(object sender, EventArgs e)
        {
            guidePaths.Clear();
            inProgressPaths.Clear();
            if (completedPaths.Count == 0)
                return;
            completedPaths.RemoveAt(completedPaths.Count-1);
            canvasView.InvalidateSurface();
        }
    }

}