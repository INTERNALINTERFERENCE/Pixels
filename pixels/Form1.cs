using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace pixels
{
    public partial class Form1 : Form
    {
        List<Bitmap> bitmaps = new List<Bitmap>();
        Random random = new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Images|*.bmp; *.png; *.jpg";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var sw = Stopwatch.StartNew();
                menuStrip1.Enabled = trackBar1.Enabled = false;
                pictureBox1.Image = null;
                bitmaps.Clear();
                var bitmap = new Bitmap(openFileDialog1.FileName);
                await Task.Run(() => { Processing(bitmap); });
                sw.Stop();
                Text = sw.Elapsed.ToString();
            }
        }

        private void Processing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currentPixelSet = new List<PixelData>(pixels.Count - pixelsInStep);

            for (int i = 1; i < trackBar1.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    int index = random.Next(pixels.Count);
                    currentPixelSet.Add(pixels[index]);
                    pixels.RemoveAt(index);

                }
                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                foreach(var pixel in currentPixelSet)
                {
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                }
                bitmaps.Add(currentBitmap);
                this.Invoke(new Action(() =>
                {
                    Text = $"{i}%";
                }));

                
            }
            bitmaps.Add(bitmap);
        }

        private List<PixelData> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<PixelData>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new PixelData
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point() { X = x, Y = y }
                    });
                }
            }
            return pixels;
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (bitmaps == null || bitmaps.Count == 0)
                return;
            // Numbering trackBar1 starts from 1, list starts from 0, so we need to substract 1
            pictureBox1.Image = bitmaps[trackBar1.Value - 1];
        }
    }
}
