using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace winmaped2 {
    public class TileViewer : Control {
        protected override Size DefaultSize {
            get {
                return new Size(64, 64);
            }
        }
        protected Vsp24Tile active_tile = null;
        protected VspObstructionTile active_obstile = null;
        private int atx = 0;
        public bool bAnimate = false;
        public enum SourceType { Vsp, Obstruction };
        public SourceType TileSourceType;
        public Vsp24Tile ActiveTile { get { return active_tile; } set { active_tile = value; repaint(); } }
        public int ActiveTileIndex { get { return atx; } set { atx = value; if (Global.ActiveMap != null) active_tile = (Vsp24Tile)Global.ActiveMap.vsp.Tiles[atx]; } }
        public VspObstructionTile ActiveObsTile { get { return active_obstile; } set { active_obstile = value; repaint(); } }
        public TileViewer() {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.Opaque, true);
        }

        private unsafe void paint(Graphics g) {
            const int BLACK = unchecked((int)0xFF000000);

            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            Bitmap bmp = Render.createBitmap(16, 16);

            using (pr2.RenderImage img = pr2.RenderImage.lockBitmap(bmp)) {
                if (TileSourceType == SourceType.Vsp) {
                    if (active_tile != null) {
                        if (!bAnimate) {
                            Render.render(img, 0, 0, active_tile.Image, true);
                        } else {
                            int frame = Global.FrameCalc.getframe(atx);
                            Render.render(img, 0, 0, Global.ActiveMap.vsp.GetTile(frame).Image, true);
                        }
                    } else {
                        img.clear(BLACK);
                    }
                } else {
                    if (active_obstile != null) {
                        Render.renderObsTile(img, 0, 0, active_obstile.Image, true, UserPrefs.ObsColor);
                    } else {
                        img.clear(BLACK);
                    }
                }
            }
            g.DrawImage(bmp, 0, 0, Width, Height);
            bmp.Dispose();
        }

        public void repaint() {
            Graphics g = this.CreateGraphics();
            paint(g);
            g.Dispose();
        }

        protected unsafe override void OnPaint(PaintEventArgs e) {
            paint(e.Graphics);
        }
    }

}