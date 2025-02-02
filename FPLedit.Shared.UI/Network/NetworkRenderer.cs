﻿using Eto.Drawing;
using Eto.Forms;
using FPLedit.Shared.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace FPLedit.Shared.UI.Network
{
    public sealed class NetworkRenderer : Drawable
    {
        private PointF mousePosition = PointF.Empty;

        private Timetable? tt;
        private readonly List<RenderBtn<Station>> panels = new List<RenderBtn<Station>>();
        private readonly Font? font;
        private readonly Pen linePen, highlightPen;
        private readonly Color systemBgColor, systemTextColor;

        public static readonly Keys[] DispatchableKeys = { Keys.R, Keys.S, Keys.Escape, Keys.Equal /* Plus */, Keys.Minus, Keys.Add, Keys.Subtract };

        protected override void Dispose(bool disposing)
        {
            if (font is { IsDisposed: false })
                font.Dispose();
            // ReSharper disable ConstantConditionalAccessQualifier
            linePen?.Dispose();
            highlightPen?.Dispose();
            // ReSharper restore ConstantConditionalAccessQualifier
        }

        private bool IsNetwork => tt?.Type == TimetableType.Network;
        
        private bool _setPanCenterEnabled = true;
        public bool SetPanCenterEnabled
        {
            get => _setPanCenterEnabled;
            set { _setPanCenterEnabled = value; Invalidate(); }
        }

        private bool _stationMovingEnabled = true;
        public bool StationMovingEnabled
        {
            get => _stationMovingEnabled;
            set { _stationMovingEnabled = value; Invalidate(); }
        }
        private string? _fixedStatusString = null;
        public string? FixedStatusString
        {
            get => _fixedStatusString;
            set { _fixedStatusString = value; Invalidate(); }
        }
        private bool _disableTopBorder = false;
        public bool DisableTopBorder
        {
            get => _disableTopBorder;
            set { _disableTopBorder = value; Invalidate(); }
        }
        private int _selectedRoute = Timetable.LINEAR_ROUTE_ID;
        public int SelectedRoute
        {
            get => _selectedRoute;
            set { _selectedRoute = value; Invalidate(); }
        }
        private bool _highlightBetween;
        public bool HighlightBetweenStations
        {
            get => _highlightBetween;
            set { _highlightBetween = value; Invalidate(); }
        }

        private PathData? _highlightedPath;

        public void SetHighlightedPath(IEnumerable<Station>? stations)
        {
            var stas = stations ?? Array.Empty<Station>();
            _highlightedPath = new PathData(tt!, stas);
            Invalidate();
        }

        private PointF _pan = PointF.Empty;
        public PointF Pan
        {
            get => _pan;
            set { _pan = value; Invalidate(); }
        }

        private float _zoom = 1;
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = Math.Max(Math.Min(value, 2f), 0.5f);
                Invalidate();
            }
        }

        private readonly StationCanvasPositionHandler handler;
        private Dictionary<Station, Point>? stapos;
        private Route[]? routes;

        public event EventHandler? StationClicked;
        public event EventHandler? StationRightClicked;
        public event EventHandler? StationDoubleClicked;
        public event EventHandler<EventArgs<int>>? NewRouteAdded;
        public event EventHandler? StationMoveEnd;

        private const int OFFSET_X = 20;
        private const int OFFSET_Y = 50;
        private readonly Point OFFSET = new Point(OFFSET_X, OFFSET_Y);

        private Station? tmp_sta;
        private float tmp_km;
        private Modes mode;

        public NetworkRenderer()
        {
            systemBgColor = SystemColors.ControlBackground;
            systemTextColor = SystemColors.ControlText;
            
            font = new Font(FontFamilies.SansFamilyName, 8);
            linePen = new Pen(systemTextColor, 2f);
            highlightPen = new Pen(Colors.Red, 2f);
            handler = new StationCanvasPositionHandler();

            MouseDown += (_, _) => PlaceStation();
            KeyDown += (_, e) => DispatchKeystroke(e);
        }

        public void SetTimetable(Timetable? newTt)
        {
            tt = newTt;
            routes = tt?.GetRoutes();
            if (tt != null)
            {
                if (tt.Type == TimetableType.Linear)
                    stapos = handler.GenerateLinearPoints(tt, ClientSize.Width);
                else
                    stapos = handler.LoadNetworkPoints(tt);
            }

            _highlightedPath = PathData.Empty(tt!);

            this.Invalidate();
        }

        public void ReloadTimetable()
        {
            SetTimetable(tt);
            Invalidate();
        }

        #region Drawing
        protected override void OnPaint(PaintEventArgs e)
        {
            this.SuspendLayout();
            // Reset
            e.Graphics.Clear(systemBgColor);
            panels.Clear();

            e.Graphics.SaveTransform();
            e.Graphics.TranslateTransform(_pan);
            e.Graphics.ScaleTransform(_zoom);
            DrawNetwork(e.Graphics, - _pan * 1/_zoom, (- _pan + Bounds.Size ) * 1/_zoom);
            e.Graphics.RestoreTransform();
            
            // Draw Status strings & border on top of network
            DrawStatus(e.Graphics);
            DrawBorder(e.Graphics);

            ResumeLayout();
            base.OnPaint(e);
        }

        private void DrawNetwork(Graphics g, PointF leftTop, PointF bottomRight)
        {
            if (routes == null || routes.Length == 0)
                return;
            
            Rectangle rec = new Rectangle((Point)leftTop, (Point)bottomRight);

            Station? lastSta = null;
            foreach (var r in routes)
            {
                Point? lastP = null;
                foreach (var sta in r.Stations)
                {
                    var pos = stapos![sta];
                    var p = OFFSET + pos;
                    var x = OFFSET_X + pos.X;
                    var y = OFFSET_Y + pos.Y;
                    bool doRender = rec.Contains(x, y);

                    // Render text only when it is (reasonably) inside viewport
                    if (doRender || rec.Intersects(new Rectangle(p, p + new Point(500, 500))))
                    {
                        var text = sta.SName + " (";
                        foreach (var ri in sta.Routes)
                        {
                            var km = sta.Positions.GetPosition(ri)!.Value.ToString("0.0");
                            if (ri == SelectedRoute && sta.Routes.Length > 1)
                                km = "▶" + km;
                            text += km + "|";
                        }

                        text = text.Substring(0, text.Length - 1) + ")";
                        
                        g.SaveTransform();
                        g.TranslateTransform(x + 6, y + 7);
                        g.RotateTransform(60);
                        g.DrawText(font, systemTextColor, new Point(0, 0), text);
                        g.RestoreTransform();
                    }

                    if (lastP.HasValue && lastSta != null && (rec.Intersects(new Rectangle(OFFSET + lastP.Value, p)) || rec.Intersects(new Rectangle(p, OFFSET + lastP.Value))))
                    {
                        var tPen = GetLinePen(r.Index, sta, lastSta);
                        g.DrawLine(tPen, p, OFFSET + lastP.Value);
                    }

                    lastP = pos;
                    lastSta = sta;

                    if (!doRender) continue;
                    
                    var panelColor = _highlightedPath!.ContainsStation(sta) ? Colors.Red : Colors.Gray;
                    RenderBtn<Station>? args = panels.FirstOrDefault(pa => pa.Tag == sta);
                    if (args == null)
                    {
                        args = new RenderBtn<Station>(sta, new Point(x - 5, y - 5), new Size(10, 10), panelColor);
                        panels.Add(args);
                        // Wire events
                        switch (mode)
                        {
                            case Modes.Normal: ApplyNormalMode(args, sta); break;
                            case Modes.AddRoute: ApplyAddMode(args, sta); break;
                            case Modes.JoinRoutes: ApplyJoinMode(args, sta); break;
                        }
                    }
                    args.BackgroundColor = panelColor;
                }
            }

            if (tmp_sta != null && stapos!.TryGetValue(tmp_sta, out Point point))
            {
                var x = OFFSET_X + point.X;
                var y = OFFSET_Y + point.Y;

                g.DrawLine(linePen, new Point(x, y), (mousePosition - _pan) * (1 / _zoom));

                var args = new RenderBtn<Station>(tmp_sta, new Point(x - 5, y - 5), new Size(10, 10), Colors.DarkCyan);
                panels.Add(args);
            }

            foreach (var args in panels)
                args.Draw(g);
        }

        private void DrawStatus(Graphics g)
        {
            if (!_pan.IsZero || Math.Abs(_zoom - 1) > 0.01f)
            {
                var statusL = T._("Ansicht verschoben, [R] für Reset");
                if (SetPanCenterEnabled && IsNetwork && !_pan.IsZero)
                    statusL += T._(", [S] zum Speichern");
                var sizeL = g.MeasureString(font, statusL);
                var pointL = new PointF(0, ClientSize.Height - sizeL.Height);
                g.FillRectangle(Brushes.Orange, new RectangleF(pointL, sizeL));
                g.DrawText(font, Brushes.Black, pointL, statusL);
            }

            var statusR = GetStatusString(mode);
            statusR = FixedStatusString ?? statusR;
            var sizeR = g.MeasureString(font, statusR);
            var pointR = new PointF(ClientSize.Width - sizeR.Width, ClientSize.Height - sizeR.Height);
            g.FillRectangle(Brushes.Turquoise, new RectangleF(pointR, sizeR));
            g.DrawText(font, Brushes.Black, pointR, statusR);
        }

        private void DrawBorder(Graphics g)
        {
            if (DisableTopBorder)
                return;
            using var pen = new Pen(Brushes.Black) { DashStyle = DashStyle.Parse("2,2,2,2") };
            g.DrawLine(pen, Point.Empty, new Point(ClientSize.Width, 0));
        }

        private Pen GetLinePen(int route, Station sta, Station lastSta)
        {
            if (route == SelectedRoute || (_highlightBetween && _highlightedPath!.IsDirectlyConnected(sta, lastSta)))
                return highlightPen;
            return linePen;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
        }
        #endregion

        #region Normal Mode
        private void ApplyNormalMode(RenderBtn<Station> p, Station sta)
        {
            p.DoubleClick += (_, _) => StationDoubleClicked?.Invoke(sta, EventArgs.Empty);
            p.Click += (_, _) => StationClicked?.Invoke(sta, EventArgs.Empty);
            p.RightClick += (_, _) => StationRightClicked?.Invoke(sta, EventArgs.Empty);
        }
        #endregion

        #region AddMode
        private void ApplyAddMode(RenderBtn<Station> p, Station sta)
        {
            p.Click += (_, _) => ConnectAddStation(sta);
        }

        private void ConnectAddStation(Station sta)
        {
            mode = Modes.Normal;
            var rtIdx = tt!.AddRoute(sta, tmp_sta!, 0, tmp_km);
            handler.WriteStapos(tt, stapos!);
            tmp_sta = null;

            NewRouteAdded?.Invoke(this, new EventArgs<int>(rtIdx));
            ReloadTimetable();
        }

        public void StartAddStation(Station rawSta, float km)
        {
            tmp_sta = rawSta;
            tmp_km = km;
            mode = Modes.AddRoute;

            Cursor = Cursors.Crosshair;
            this.Focus();
        }

        private void ApplyJoinMode(RenderBtn<Station> p, Station sta)
        {
            p.Click += (_, _) => ConnectJoinLines(sta);
        }

        public void StartJoinLines(float km)
        {
            tmp_km = km;
            mode = Modes.JoinRoutes;

            Cursor = Cursors.Crosshair;
            this.Focus();
        }

        private void ConnectJoinLines(Station sta)
        {
            if (sta.Routes.Contains(SelectedRoute))
                MessageBox.Show(T._("Die Verbindung konnte nicht erstellt werden, da die gewählte Zielstation sich bereits auf der Gleichen Strecke befindet!"));
            else if (!tt!.JoinRoutes(SelectedRoute, sta, tmp_km))
                MessageBox.Show(T._("Die Verbindung konnte nicht erstellt werden, da sonst Routen zusammenfallen würden!"));
            
            tmp_sta = null;
            mode = Modes.Normal;

            ReloadTimetable();
        }

        private void AbortAddStation()
        {
            if (tmp_sta == null) // Nicht benötigt
                return;

            tmp_sta = null;
            Cursor = Cursors.Default;
            mode = Modes.Normal;

            Invalidate();
        }

        private void PlaceStation()
        {
            if (tmp_sta == null || stapos!.TryGetValue(tmp_sta, out _))
                return;

            Cursor = Cursors.Default;
            var point = new Point((mousePosition - _pan) * (1 / _zoom)) - OFFSET;
            stapos[tmp_sta] = new Point(point);

            Invalidate();
        }
        #endregion

        private string GetStatusString(Modes m)
        {
            return m switch
            {
                Modes.Normal => T._("Streckennetz bearbeiten"),
                Modes.AddRoute => T._("Klicken, um Station hinzuzufügen und diese mit einer bestehenden Station zu verbinden; ESC zum Abbrechen"),
                Modes.JoinRoutes => T._("Klicken, um die Zielstation der Verbindung auzuwählen; ESC zum Abbrechen"),
                _ => ""
            };
        }

        public void DispatchKeystroke(KeyEventArgs e)
        {
            switch (e.Key) // See Dispatchable_Keys
            {
                case Keys.Escape:
                    AbortAddStation();
                    e.Handled = true;
                    break;
                case Keys.R:
                    Pan = PointF.Empty;
                    Zoom = 1f;
                    e.Handled = true;
                    break;
                case Keys.S:
                    if (SetPanCenterEnabled && tt != null && stapos != null && IsNetwork)
                    {
                        e.Handled = true;
                        var keys = stapos.Keys.ToArray();
                        foreach (var sta in keys)
                            stapos[sta] += (Point)(Pan * 1/_zoom);
                        handler.WriteStapos(tt, stapos);
                        Pan = PointF.Empty;
                    }
                    break;
                case Keys.Equal: // Plus key
                case Keys.Add:
                    Zoom += 0.1f;
                    break;
                case Keys.Minus:
                case Keys.Subtract:
                    Zoom -= 0.1f;
                    break;
            }
        }

        #region Drag'n'Drop
        private RenderBtn<Station>? draggedControl;
        private bool hasDragged = false;
        private const int CLICK_TIME = 10^6; // 0.1*10^7s, 1 tick = 10^-7 seconds
        private long lastClick = 0;
        private bool lastDoubleClick;
        private bool hasPanned = false;
        private PointF originalLocation;
        private PointF originalPan;

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            draggedControl = null;
            Cursor = Cursors.Default;
            hasDragged = false;
            hasPanned = false;

            foreach (var args in panels.ToArray())
                args.HandleDoubleClick(new Point(e.Location), new Point(_pan), _zoom);

            lastDoubleClick = true;
            base.OnMouseDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            lastClick = DateTime.Now.Ticks;

            if (!lastDoubleClick)
            {
                if (e.Buttons == MouseButtons.Alternate)
                    foreach (var args in panels.ToArray())
                        args.HandleRightClick(new Point(e.Location), new Point(_pan), _zoom);
                else if (e.Buttons == MouseButtons.Primary && StationMovingEnabled && IsNetwork)
                {
                    foreach (var args in panels.ToArray())
                    {
                        if (args.Rect.Contains(new Point((new Point(e.Location) - new Point(_pan)) * (1/_zoom))))
                        {
                            draggedControl = args;
                            Cursor = Cursors.Move;
                        }
                    }
                }

                if (e.Buttons == MouseButtons.Primary && draggedControl == null && tt != null) // Only pan if we have data & we don't drag anything
                {
                    hasPanned = true;
                    originalPan = _pan;
                    originalLocation = e.Location;
                    Cursor = Cursors.Move;
                }
            }

            lastDoubleClick = false;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            mousePosition = e.Location;
            if (StationMovingEnabled && IsNetwork && draggedControl != null)
            {
                var p = new Point(e.Location);

                // Not getting out of bounds
                if (p.X < 0)
                    p.X = 0;
                if (p.Y < 0)
                    p.Y = 0;
                if (p.X > ClientSize.Width)
                    p.X = ClientSize.Width;
                if (p.Y > ClientSize.Height)
                    p.Y = ClientSize.Height;

                draggedControl.Location = p;
                stapos![draggedControl.Tag] = new Point((p - _pan) * (1 / _zoom)) - OFFSET;
                hasDragged = true;
                Invalidate();
            }
            if (hasPanned)
            {
                _pan = originalPan + (e.Location - originalLocation) * (1 / _zoom);
                Invalidate();
            }
            if (tmp_sta != null)
                Invalidate();
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Buttons == MouseButtons.Primary && DateTime.Now.Ticks >= lastClick + CLICK_TIME)
            {
                foreach (var args in panels.ToArray())
                    args.HandleClick(new Point(e.Location), new Point(_pan), _zoom);
            }

            if (StationMovingEnabled && IsNetwork && draggedControl != null)
            {
                draggedControl = null;
                Cursor = Cursors.Default;
                handler.WriteStapos(tt!, stapos!);
                if (hasDragged)
                {
                    Invalidate();
                    StationMoveEnd?.Invoke(this, EventArgs.Empty);
                }
                hasDragged = false;
            }

            if (hasPanned)
            {
                hasPanned = false;
                Cursor = Cursors.Default;
            }

            lastClick = 0;
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Zoom += e.Delta.Height * 0.1f;
            base.OnMouseWheel(e);
        }

        #endregion

        private enum Modes
        {
            Normal,
            AddRoute,
            JoinRoutes,
        }
    }
}
