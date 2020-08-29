using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TensorFlow;
using Luxand;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace ObjectMonitor
{
    public partial class Form1 : Form
    {
        private static double MIN_SCORE_FOR_OBJECT_HIGHLIGHTING = 0.5;
        //public VideoViewerWF mVideoViewr;
       
        public string model_file;
        public string model_text;
        public TFGraph mGraph;
        bool bFirstRun = true;
        private static IEnumerable<CatalogItem> mCatalog;
        bool usbCamera = true;
        public string db_addr;
        public string db_username;
        public string db_password;
        public string ipcam_addr;
        public string ipcam_username;
        public string ipcam_password;
        Thread mThread;
        bool CameraOpened = false;
        bool needClose = false;
        TFSession session;

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            System.Reflection.PropertyInfo aProp =
                  typeof(System.Windows.Forms.Control).GetProperty(
                        "DoubleBuffered",
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

        public Form1()
        {
            try
            {
                InitializeComponent();
                label1.Visible = false;
                if (FSDK.FSDKE_OK != FSDK.ActivateLibrary("gyYgVWQTSzjiuGB/hH8dKgg0QrrIuhoHdfUCzD9rY+vru3WRZsaezTX6YWj9osdI/cmxY1NSdLkyWuugMPCxUG7/xNLegHLeaUpzVyKpDkaWL8tJIUsIL7xv9bhmgifPbAyTDuxF3VGxXmHkv/L/MStf9kdXV/A1vVvT93QC4vQ="))
                {
                    MessageBox.Show("Please run the License Key Wizard (Start - Luxand - FaceSDK - License Key Wizard)", "Error activating FaceSDK", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }

                FSDK.InitializeLibrary();

                SetDoubleBuffered(pictureBox1);
                model_file = "frozen_inference_graph.pb";
                model_text = "mscoco_complete_label_map.pbtxt";
    
                LoadingForm loadingPannel = new LoadingForm();
                mGraph = new TFGraph();
                Thread loadingThread = new System.Threading.Thread(delegate ()
                {
                    if (loadingPannel.InvokeRequired)
                    {
                        loadingPannel.Invoke(new Action<string>(s =>
                          {
                              loadingPannel.ShowDialog();
                          }), " ");
                    }
                    else
                    {
                        loadingPannel.ShowDialog();
                    }
                });
                try
                {
                    loadingThread.Start();
                    mCatalog = CatalogUtil.ReadCatalogItems(model_text);
                    var model = File.ReadAllBytes(model_file);
                    mGraph.Import(new TFBuffer(model));
                    session = new TFSession(mGraph);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    if (loadingPannel.InvokeRequired)
                    {
                        loadingPannel.Invoke(new MethodInvoker(delegate ()
                        {
                            loadingPannel.Close();
                        }), " ");
                    }
                    else
                    {
                        loadingPannel.Close();
                    }
                    
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        
        int cameraHandle = 0;
        String cameraName;
        private void startDetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            bFirstRun = true;
            needClose = true;
            if (mThread != null)
                mThread.Abort();

            if (CameraOpened)
            {
                CameraOpened = false;

                if (FSDK.FSDKE_OK != FSDKCam.CloseVideoCamera(cameraHandle))
                {
                    MessageBox.Show("Error closing camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }

            if (usbCamera)
            {
                FSDKCam.InitializeCapturing();

                string[] cameraList;
                int count;
                FSDKCam.GetCameraList(out cameraList, out count);

                if (0 >= count)
                {
                    MessageBox.Show("Please attach a camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Visible = false;
                    Application.Exit();
                }
                cameraName = cameraList[0];

                FSDKCam.VideoFormatInfo[] formatList;
                FSDKCam.GetVideoFormatList(ref cameraName, out formatList, out count);

                int r = FSDKCam.OpenVideoCamera(ref cameraName, ref cameraHandle);
                if (r != FSDK.FSDKE_OK)
                {
                    MessageBox.Show("Error opening the first camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Visible = false;
                    //Application.Exit();
                }
                else
                {
                    CameraOpened = true;
                }

            }
            else
            {


                if (FSDK.FSDKE_OK != FSDKCam.OpenIPVideoCamera(FSDKCam.FSDK_VIDEOCOMPRESSIONTYPE.FSDK_MJPEG, ipcam_addr, ipcam_username, ipcam_password, 50, ref cameraHandle))
                {
                    MessageBox.Show("Error opening IP camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    label1.Visible = false;
                    //Application.Exit();
                }
                else
                {
                    CameraOpened = true;
                }
            }
            needClose = false;

            mThread = new Thread(new ThreadStart(MainLoop));
            mThread.Start();
        }

        private byte[] GetRGBValues(Bitmap bmp)
        {

            // Lock the bitmap's bits. 
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
             bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
             bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;
            byte[] rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes); bmp.UnlockBits(bmpData);

            return rgbValues;
        }

        public void MainLoop()
        {
            Int32 imageHandle = 0;

            while (!needClose)
            {
                if (!CameraOpened)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if (FSDK.FSDKE_OK != FSDKCam.GrabFrame(cameraHandle, ref imageHandle)) // grab the current frame from the camera
                {
                    Application.DoEvents();
                    continue;
                }

                FSDK.CImage image = new FSDK.CImage(imageHandle);
                Image frameImage = image.ToCLRImage();
                Graphics gr = Graphics.FromImage(frameImage);
                try
                {
                    MemoryStream ms = new MemoryStream();
                    frameImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] vs = ms.ToArray();
                    using (var tensor = ImageUtil.CreateTensorFromMemoryJpg(vs, TFDataType.UInt8))
                    {
                        var runner = session.GetRunner();

                        runner
                            .AddInput(mGraph["image_tensor"][0], tensor)
                            .Fetch(
                            mGraph["detection_boxes"][0],
                            mGraph["detection_scores"][0],
                            mGraph["detection_classes"][0]);

                        var output = runner.Run();

                        var boxes = (float[,,])output[0].GetValue(jagged: false);
                        var scores = (float[,])output[1].GetValue(jagged: false);
                        var classes = (float[,])output[2].GetValue(jagged: false);
                        // var num = (float[])output[3].GetValue(jagged: false);
                        if (bFirstRun)
                        {
                            if (label1.InvokeRequired)
                            {
                                label1.Invoke(new Action<string>(s =>
                                {
                                    label1.Visible = false;
                                }), " ");
                            }
                            else
                            {
                                label1.Visible = false;
                            }

                            bFirstRun = false;
                        }
                        lock (lockObject)
                        {
                            DrawBoxes(boxes, scores, classes, frameImage);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            FSDKCam.CloseVideoCamera(cameraHandle);
            FSDKCam.FinalizeCapturing();
        }

        public delegate void SafeCall(Image img);
        public void SetPictureImage(Image img)
        {
            if (pictureBox1.InvokeRequired)
            {
                try
                {
                    var d = new SafeCall(SetPictureImage);
                    Invoke(d, new object[] { img });
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            else
            {
                pictureBox1.Image = img;
            }
        }

        private void DrawBoxes(float[,,] boxes, float[,] scores, float[,] classes , Image img)
        {
            var x = boxes.GetLength(0);
            var y = boxes.GetLength(1);
            var z = boxes.GetLength(2);
            bool anyDetected = false;
            float ymin = 0, xmin = 0, ymax = 0, xmax = 0;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    if (scores[i, j] < MIN_SCORE_FOR_OBJECT_HIGHLIGHTING) continue;

                    for (int k = 0; k < z; k++)
                    {
                        var box = boxes[i, j, k];
                        switch (k)
                        {
                            case 0:
                                ymin = box;
                                break;
                            case 1:
                                xmin = box;
                                break;
                            case 2:
                                ymax = box;
                                break;
                            case 3:
                                xmax = box;
                                break;
                        }

                    }

                    int value = Convert.ToInt32(classes[i, j]);
                    anyDetected = true;
                    CatalogItem catalogItem = mCatalog.FirstOrDefault(item => item.Id == value);
                    AddBox(img, xmin, xmax, ymin, ymax, $"{catalogItem.DisplayName} : {(scores[i, j] * 100).ToString("0")}%");
                    //Console.Write($"{catalogItem.DisplayName} : {(scores[i, j] * 100).ToString("0")}%\n");
                }
            }

            if (anyDetected == false)
            {
                pictureBox1.Image = img;
            }

        }


        private object lockObject = new object();
        private string _fontFamily ;
        private float _fontSize;

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        private void AddBox(Image img , float xmin, float xmax, float ymin, float ymax, string text = "", string colorName = "red")
        {

            var _graphics = Graphics.FromImage(img);

            var left = xmin * img.Width;
            var right = xmax * img.Width;
            var top = ymin * img.Height;
            var bottom = ymax * img.Height;

            _fontFamily = "Ariel";
            _fontSize = 12;

            //var imageRectangle = new System.Drawing.Rectangle(new Point(0, 0), new Size(img.Width, img.Height));
            
            Color color = Color.FromName(colorName);
            Brush brush = new SolidBrush(color);
            Pen pen = new Pen(brush);

            _graphics.DrawRectangle(pen, left, top, right - left, bottom - top);
            var font = new Font(_fontFamily, _fontSize);
            SizeF size = _graphics.MeasureString(text, font);
            _graphics.DrawString(text, font, brush, new PointF(left, top - size.Height));
            /* if ((top - size.Height) <= 0)
             {
                 Console.Write("Bound out y:" + (top - size.Height));
             }*/

            /*  if (pictureBox1.InvokeRequired)
                 {
                     pictureBox1.Invoke(new MethodInvoker( delegate()
                     {
                         pictureBox1.Image = img;
                     }), " ");
                 }
                 else
                 {
                     pictureBox1.Image = img;
                 }*/

            //pictureBox1.Image = img;
            _graphics.Dispose();
            //SetPictureImage(img);
            
            using (Graphics picG = pictureBox1.CreateGraphics())
            {
                //var wrapMode = new ImageAttributes();
                //var destRect = new System.Drawing.Rectangle(0, 0, pictureBox1.Width , pictureBox1.Height);
                //wrapMode.SetWrapMode(WrapMode.Tile);
                //picG.DrawImage(img, destRect, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, wrapMode);
                Bitmap resizedImage = ResizeBitmap(img, pictureBox1.Width, pictureBox1.Height, InterpolationMode.NearestNeighbor);
                //Bitmap resizedImage = new Bitmap(img, pictureBox1.Width, pictureBox1.Height);
                picG.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                picG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                picG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                picG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                picG.DrawImageUnscaled(resizedImage, 0, 0, resizedImage.Width, resizedImage.Height);
                //picG.DrawImage(img, new Point(0,0));
                picG.Dispose();
                
            }

           

        }

        public static Bitmap ResizeBitmap(Image source, int width , int height , InterpolationMode quality)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            // Figure out the new size.
            //var width = (int)(source.Width * scale);
            //var height = (int)(source.Height * scale);

            // Create the new bitmap.
            // Note that Bitmap has a resize constructor, but you can't control the quality.
            var bmp = new Bitmap(width, height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = quality;
                g.DrawImage(source, new System.Drawing.Rectangle(0, 0, width, height));
                g.Save();
            }

            return bmp;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            needClose = true;
        }

        private void uSBCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            iPCameraToolStripMenuItem.CheckState = CheckState.Unchecked;
            uSBCameraToolStripMenuItem.CheckState = CheckState.Checked;
            usbCamera = true;
        }

        private void iPCameraToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uSBCameraToolStripMenuItem.CheckState = CheckState.Unchecked;
            iPCameraToolStripMenuItem.CheckState = CheckState.Checked;
            usbCamera = false;
            Form2 frm2 = new Form2(this, false);
            frm2.Show();
        }

        enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows 
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

    }
}
