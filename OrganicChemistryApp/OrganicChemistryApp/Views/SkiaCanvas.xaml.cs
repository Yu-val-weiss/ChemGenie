using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using TouchTracking;
using Xamarin.Forms;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Chemicals;
using Element = Chemicals.Element;
using HttpRequestException = System.Net.Http.HttpRequestException;
using OrganicChemistryApp.Services;
using Xamarin.Essentials;

namespace OrganicChemistryApp.Views
{
    public class BondPath : SKPath
    {
        public BondOrder Order = BondOrder.Single;
    }

    public partial class SkiaCanvas : ContentPage
    {

        Dictionary<long, BondPath> inProgressPaths = new Dictionary<long, BondPath>();
        Dictionary<SKPoint, List<BondPath>> guidePaths = new Dictionary<SKPoint, List<BondPath>>();
        List<BondPath> completedPaths = new List<BondPath>();
        Dictionary<SKPoint, Element> diffElements = new Dictionary<SKPoint, Element>();
        Stack<string> undoStack = new Stack<string>();
        private readonly ElementBuilder eb;

        #region SKPaints
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColor.Parse("909090"),
            StrokeWidth = 10,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };

        SKPaint paint2 = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.White,
            StrokeWidth = 15,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
        };

        SKPaint paint3 = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColor.Parse("909090"),
            StrokeWidth = 30,
            StrokeCap = SKStrokeCap.Butt,
            StrokeJoin = SKStrokeJoin.Round,
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
            Assembly assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith("Elements.xml"));

            Picker.ItemsSource = new List<string>{"Single","Double","Triple"};
            var stream = assembly.GetManifestResourceStream(resourceName);
            eb = new ElementBuilder(stream);
        }

        void TouchEffect_OnTouchAction(object sender, TouchActionEventArgs args)
        {
            switch (args.Type)
            {
                case TouchActionType.Pressed:
                    if (!inProgressPaths.ContainsKey(args.Id))
                    {
                        guidePaths.Clear();
                        BondPath path = new BondPath();
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

                        var compp = new List<SKPoint[]>();

                        if (pixel != ConvertToPixel(args.Location))
                        {
                            foreach (var p in completedPaths)
                                compp.Add(p.GetLine());
                            guidePaths[pixel].RemoveAll(pth => compp.Contains(pth.GetLine()) || compp.Contains(ReverseLine(pth)));
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
                        BondPath path = inProgressPaths[args.Id];
                        var firstPoint = path[0];
                        path.Rewind();
                        path.MoveTo(firstPoint);
                        path.LineTo(ConvertToPixel(args.Location));

                        if (guidePaths.ContainsKey(firstPoint))
                        {

                            BondPath linqPath = guidePaths[firstPoint]
                                .FirstOrDefault(pth => SKPoint.Distance(pth.LastPoint, path.LastPoint) < 30);
                            if (linqPath != null)
                            {
                                guidePaths.Remove(linqPath[0]);
                                linqPath.Order = BondOrderFromPicker();
                                completedPaths.Add(linqPath);
                                undoStack.Push("path");
                                MakeNewPathFromPoint(args, linqPath.LastPoint);
                                SuggestGuidePaths(linqPath.LastPoint);
                                SuggestAltGuidePaths(linqPath.LastPoint);
                            }
                           
                        }
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
            foreach (BondPath path in guidePaths.Values.SelectMany(x => x))
            {
                canvas.DrawPath(path, guidePaint);
            }

            foreach (BondPath path in completedPaths)
            {
                switch (path.Order)
                {
                    case BondOrder.Single:
                        canvas.DrawPath(path,paint);
                        break;
                    case BondOrder.Double:
                        /*pth.MoveTo(path[0].X + 10, path[0].Y + 20f);
                        pth.LineTo(path.LastPoint.X - 10, path.LastPoint.Y + 20);
                        canvas.DrawPath(pth, paint);*/
                        canvas.DrawPath(path,paint3);
                        canvas.DrawPath(path,paint2);
                        break;
                    case BondOrder.Triple:
                        /*pth.MoveTo(path[0].X + 10, path[0].Y + 20f);
                        pth.LineTo(path.LastPoint.X - 10, path.LastPoint.Y + 20);
                        canvas.DrawPath(pth, paint);
                        pth.MoveTo(path[0].X + 10, path[0].Y - 20f);
                        pth.LineTo(path.LastPoint.X - 10, path.LastPoint.Y - 20);
                        canvas.DrawPath(pth, paint);*/
                        canvas.DrawPath(path,paint3);
                        canvas.DrawPath(path,paint2);
                        elPaint.Color = SKColor.Parse("909090");
                        canvas.DrawPath(path,elPaint);
                        break;
                }
            }


            foreach (BondPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
            }

            foreach (var element in diffElements)
            {
                var colour = SKColor.Parse(element.Value.Colour);
                elPaint.Color = colour;
                canvas.DrawCircle(element.Key, 50, elPaint);
                canvas.DrawCircle(element.Key,47.5f,elFillPaint);
                SKPaint textPaint = new SKPaint
                {
                    Color = colour,
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

        SKPoint[] ReverseLine(BondPath bp)
        {
            var x = bp.GetLine();
            x.Reverse();
            return x;
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
            BondPath newPath = new BondPath();
            newPath.MoveTo(pt);
            inProgressPaths.Add(args.Id, newPath);
        }

        private void SuggestGuidePaths(SKPoint pixel)
        {
            float x = pixel.X;
            float y = pixel.Y;
            var pathList = new List<BondPath>();
            const float len = 200;
            const double ang = 0.5477400137;

            var path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x + len * (float) Math.Cos(ang), y + len * (float) Math.Sin(ang));
            pathList.Add(path);

            path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x + len * (float) Math.Cos(ang), y - len * (float) Math.Sin(ang));
            pathList.Add(path);

            path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x - len * (float) Math.Cos(ang), y + len * (float) Math.Sin(ang));
            pathList.Add(path);

            path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x - len * (float) Math.Cos(ang), y - len * (float) Math.Sin(ang));
            pathList.Add(path);

            guidePaths.Add(pixel, pathList);

        }

        private void SuggestAltGuidePaths(SKPoint pixel)
        {
            float x = pixel.X;
            float y = pixel.Y;
            var pathList = new List<BondPath>();
            const float len = 200;

            var path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x, y + len );
            pathList.Add(path);

            path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x, y - len );
            pathList.Add(path);

            path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x + len , y);
            pathList.Add(path);

            path = new BondPath();
            path.MoveTo(x, y);
            path.LineTo(x - len, y);
            pathList.Add(path);

            guidePaths[pixel].AddRange(pathList);
        }

        private BondOrder BondOrderFromPicker()
        {
            switch (Picker.SelectedIndex)
            {
                case -1:
                case 0:
                    return BondOrder.Single;
                case 1:
                    return BondOrder.Double;
                case 2:
                    return BondOrder.Triple;
            }
            return BondOrder.Single;
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
                var x = diffElements.FirstOrDefault(dif => dif.Key.ToString() == undoStack.Peek()).Key;
                undoStack.Pop();
                if (x != default(SKPoint))
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

                var ele = eb.CreateElement(inpStr);
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
            catch (NullReferenceException)
            {
                await DisplayAlert("Error", "Please input a valid Symbol", "OK");
                DiffChemical_OnClicked(sender, e);
            }
           
        }

        private async void Chemical_Searched(object sender, EventArgs e)
        {
            if (!await CheckNetworkStatus() || completedPaths.Count == 0)
                return;

            var carbon = eb.CreateElement("C");
            var atomdict = new Dictionary<SKPoint,AtomNode>();
            Molecule mole = new Molecule();


            foreach (var x in completedPaths)
            {
                var line = x.GetLine();

                var first = line[0];
                var second = line[1];


                if (!atomdict.ContainsKey(first))
                {
                    var atom = diffElements.ContainsKey(first)
                        ? new AtomNode(diffElements[first])
                        : new AtomNode(carbon);
                    atomdict.Add(first,atom);
                    mole = new Molecule(atom);
                }

                if (!atomdict.ContainsKey(second))
                {
                    var keys = atomdict.Keys;
                    var point = keys.FirstOrDefault(pt => Math.Abs(SKPoint.Distance(pt, second)) < 3);
                    if (point != default(SKPoint))
                    {
                        second = point;
                    }
                    else
                    {
                        var atom2 = diffElements.ContainsKey(second)
                            ? new AtomNode(diffElements[second])
                            : new AtomNode(carbon);
                        atomdict.Add(second, atom2);
                    }
                    
                }

                mole.AddBond(x.Order, atomdict[first], atomdict[second]);

            }

            var smiles = mole.ToSMILES().Replace('=', '£');
            var mass = mole.GetMolecularMass();
            await Shell.Current.GoToAsync($"resultPage?mass={mass}&search={smiles}");
        }

        private async void SearchBar_OnSearchButtonPressed(object sender, EventArgs e)
        {
            if (!await CheckNetworkStatus())
                return;

            SearchBar searchBar = (SearchBar) sender;
            Indicator.IsRunning = true;
            var dict = new Dictionary<string,string>();
            var prq = new PugRestQuery(searchBar.Text);

            try
            {
                var response = await prq.GetStringFromIUPAC();
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(response);
                if (xmlDocument.DocumentElement != null)
                {
                    var c = xmlDocument.DocumentElement.FirstChild.ChildNodes;
                    foreach (XmlNode x in c)
                    {
                        if (x.Name == "CID") continue;
                        dict.Add(x.Name, x.InnerText);
                    }
                }
            }
            catch (Exception exception)
            {
                /*if (exception.Message.Contains("404"))
                {
                    await DisplayAlert("Error", "Invalid name", "OK");
                    Indicator.IsRunning = false;
                    return;
                }*/
                await DisplayAlert("Error", exception.Message, "OK");
                Indicator.IsRunning = false;
                return;
            }
            
            var mass = $"{Math.Round(double.Parse(dict["MolecularWeight"]), 1):0.0}";
            var smiles = dict["CanonicalSMILES"];
            smiles = smiles.Replace('=', '£');

            Indicator.IsRunning = false;
            await Shell.Current.GoToAsync($"resultPage?mass={mass}&search={Uri.EscapeDataString(smiles)}&title={Uri.EscapeDataString(searchBar.Text)}");
        }

        private void SearchBar_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            guidePaths.Clear();
            canvasView.InvalidateSurface();
        }

        private async Task<bool> CheckNetworkStatus()
        {
            var current = Connectivity.NetworkAccess;
            switch (current)
            {
                case NetworkAccess.Internet:
                    return true;
                default:
                    await DisplayAlert("Error", "Device must be connected to the internet", "OK");
                    return false;
            }
        }
    }

}