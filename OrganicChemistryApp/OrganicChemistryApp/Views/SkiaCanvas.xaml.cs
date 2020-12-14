using System;
using System.Collections.Generic;
using System.IO;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Forms;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Chemicals;
using Xamarin.Forms.Xaml;
using Element = Chemicals.Element;

namespace OrganicChemistryApp.Views
{
    public partial class SkiaCanvas : ContentPage
    {

        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        Dictionary<SKPoint, List<SKPath>> guidePaths = new Dictionary<SKPoint, List<SKPath>>();

        List<SKPath> completedPaths = new List<SKPath>();
        Dictionary<SKPath, BondOrder> bondOrders = new Dictionary<SKPath, BondOrder>();
        Dictionary<SKPoint, Element> diffElements = new Dictionary<SKPoint, Element>();

        Stack<string> undoStack = new Stack<string>();
        Assembly assembly = Assembly.GetExecutingAssembly();
        private string resourceName;

        #region SKPaints
    

            
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.Blue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        SKPaint guidePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.CornflowerBlue,
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            PathEffect = SKPathEffect.CreateDash(new float[] {15, 15}, 20)
        };
        SKPaint elPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.DarkCyan,
            StrokeWidth = 5,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        SKPaint elFillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = SKColors.White,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        #endregion


        public SkiaCanvas()
        {
            InitializeComponent();
            resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("Elements.xml"));
            
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
                                    SuggestGuidePaths(pixel);
                                    SuggestAltGuidePaths(pixel);
                                    break;
                                }
                                if (SKPoint.Distance(pth.LastPoint, pixel) < 50)
                                {
                                    pixel = pth.LastPoint;
                                    SuggestGuidePaths(pixel);
                                    SuggestAltGuidePaths(pixel);
                                    break;
                                }
                            }
                        }

                        path.MoveTo(pixel);
                        inProgressPaths.Add(args.Id, path);

                        var gp = new List<SKPoint>();
                        var cp = new List<SKPoint>();

                        if (pixel != ConvertToPixel(args.Location))
                        {
                            foreach (var p in guidePaths[pixel])
                                gp.Add(p.LastPoint);
                            foreach (var p in completedPaths)
                                cp.AddRange(p.GetLine());
                            var intersect = cp.Intersect(gp).ToList();
                            guidePaths[pixel].RemoveAll(pth => intersect.Contains(pth.LastPoint));
                        }

                        if (completedPaths.Count == 0)
                        {
                            SuggestGuidePaths(pixel);
                            SuggestAltGuidePaths(pixel);
                        }
                            

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

                        if (guidePaths.ContainsKey(firstPoint))
                        {

                            SKPath linqPath = guidePaths[firstPoint]
                                .FirstOrDefault(pth => SKPoint.Distance(pth.LastPoint, path.LastPoint) < 30);
                            if (linqPath != null)
                            {
                                guidePaths.Remove(linqPath[0]);
                                completedPaths.Add(linqPath);
                                undoStack.Push("path");
                                MakeNewPathFromPoint(args, linqPath.LastPoint);
                                SuggestGuidePaths(linqPath.LastPoint);
                                SuggestAltGuidePaths(linqPath.LastPoint);
                            }
                           
                        }
                        /*if (completedPaths.Count > 2)
                        {
                            SKPath linqPathF = completedPaths.FirstOrDefault(pth =>
                                SKPoint.Distance(pth[0], path.LastPoint) < 60);
                            SKPath linqPathL = completedPaths.FirstOrDefault(pth =>
                                SKPoint.Distance(pth.LastPoint, path.LastPoint) < 60);
                            if (linqPathF != null)
                            {
                                guidePaths.Remove(linqPathF[0]);
                                completedPaths.Add(linqPathF);
                                undoStack.Push("path");
                                MakeNewPathFromPoint(args, linqPathF.LastPoint);
                            }
                            else if (linqPathL != null)
                            {
                                guidePaths.Remove(linqPathL[0]);
                                completedPaths.Add(linqPathL);
                                undoStack.Push("path");
                                MakeNewPathFromPoint(args, linqPathL.LastPoint);
                            }
                        }*/

                        

                        canvasView.InvalidateSurface();
                    }

                    break;

                case TouchActionType.Released:
                    if (inProgressPaths.ContainsKey(args.Id))
                    {
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

            foreach (var element in diffElements)
            {
                canvas.DrawCircle(element.Key, 50, elPaint);
                canvas.DrawCircle(element.Key,47.5f,elFillPaint);

                string str = element.Value.Symbol;
                SKPaint textPaint = new SKPaint
                {
                    Color = SKColors.DarkCyan,
                    TextSize = 80,
                    TextAlign = SKTextAlign.Center
                };
                SKPoint point = element.Key;
                var textY = point.Y + ((-textPaint.FontMetrics.Ascent + textPaint.FontMetrics.Descent) / 2 - textPaint.FontMetrics.Descent);
                canvas.DrawText(element.Value.Symbol,point.X,textY,textPaint);
            }

        }

        SKPoint ConvertToPixel(TouchTrackingPoint pt)
        {
            return new SKPoint((float) (canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                (float) (canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }

        private void ClearCanvas_OnClicked(object sender, EventArgs e)
        {
            inProgressPaths.Clear();
            completedPaths.Clear();
            guidePaths.Clear();
            diffElements.Clear();
            undoStack.Clear();
            canvasView.InvalidateSurface();
            Title = "Draw";
        }


        private void MakeNewPathFromPoint(TouchActionEventArgs args, SKPoint pt)
        {
            inProgressPaths.Remove(args.Id);
            SKPath newPath = new SKPath();
            newPath.MoveTo(pt);
            inProgressPaths.Add(args.Id, newPath);
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

            var path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x, y + len );
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x, y - len );
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x + len , y);
            pathList.Add(path);

            path = new SKPath();
            path.MoveTo(x, y);
            path.LineTo(x - len, y);
            pathList.Add(path);

            guidePaths[pixel].AddRange(pathList);
        }

        private void Undo_OnClicked(object sender, EventArgs e)
        {
            guidePaths.Clear();
            inProgressPaths.Clear();
            if (undoStack.Count == 0)
                return;
            if (undoStack.Peek() == "path")
            {
                undoStack.Pop();
                completedPaths.RemoveAt(completedPaths.Count - 1);
            }
            else
            {
                var x = diffElements.First(dif => dif.Key.ToString() == undoStack.Peek()).Key;
                undoStack.Pop();
                diffElements.Remove(x);
            }
            
            canvasView.InvalidateSurface();
        }

        private async void DiffChemical_OnClicked(object sender, EventArgs e)
        {
            if (guidePaths.Count == 0)
            {
                await DisplayAlert("Error", "Please select an atom to replace first", "OK");
                return;
            }


            string inpStr = await DisplayPromptAsync("Add atom", "Please type the symbol of the atom you want to add", maxLength: 2, keyboard: Keyboard.Text);
            if (inpStr is null)
                return;

            try
            {
                var stream = assembly.GetManifestResourceStream(resourceName);
                var ele = new Element(inpStr, stream);
                var key = guidePaths.Last().Key;
                if (!diffElements.ContainsKey(key))
                    diffElements.Add(key, ele);
                else
                {
                    diffElements[key] = ele;
                }
                undoStack.Push(guidePaths.Last().Key.ToString());
                canvasView.InvalidateSurface();
            }
            catch (NullReferenceException exception)
            {
                await DisplayAlert("Error", "Please input a valid Symbol", "OK");
                DiffChemical_OnClicked(sender, e);
            }
           
        }

        private void Chemical_Searched(object sender, EventArgs e)
        {
            if (completedPaths.Count == 0)
                return;
            Stream stream = assembly.GetManifestResourceStream(resourceName);
            var carbon = new Element("C", stream);
            var atomdict = new Dictionary<SKPoint,AtomNode>();
            foreach (var x in completedPaths)
            {
                //BondOrder order = bondOrders[x];
                var line = x.GetLine();

                var first = line[0];
                var second = line[1];


                if (!atomdict.ContainsKey(first))
                {
                    var atom = diffElements.ContainsKey(first)
                        ? new AtomNode(diffElements[first])
                        : new AtomNode(carbon);
                    atomdict.Add(first,atom);
                }

                if (!atomdict.ContainsKey(second))
                {
                    var atom2 = diffElements.ContainsKey(second)
                        ? new AtomNode(diffElements[second])
                        : new AtomNode(carbon);
                    atomdict[first].AddBond(atom2, BondOrder.Single);
                    atomdict.Add(second, atom2);
                }

                else if (atomdict.ContainsKey(second))
                {
                    atomdict[first].AddBond(atomdict[second], BondOrder.Single);
                }
                
            }
            var mole = new Molecule(atomdict.First().Value);
            atomdict.Remove(atomdict.First().Key);
            mole.Atoms.AddRange(atomdict.Values);
            Title = mole.ToSMILES();
        }

        private void RingButton_OnClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}