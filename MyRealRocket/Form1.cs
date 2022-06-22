using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Threading;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using GMap.NET.Projections;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Drawing.Imaging;
using Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL;




namespace MyRealRocket
{
    public partial class Form1 : Form
    {
        public static string data;
        string[] splitted_data;

        float x = 0, y = 0, z = 0;
        bool cx = false, cy = false, cz = false;
        double enlem = 0;
        double boylam = 0;




        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            GL.ClearColor(Color.FromArgb(26, 26, 26));//Color.FromArgb(143, 212, 150)   // 3D ROKETİN ARKA PLAN RENGİNİ DEĞİŞTİRDİK

            //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "d/M/yyyy HH:mm:ss";




        }



        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();  // Close button
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;  // Minimize buton
        }



        private void PortComboBox_DropDown(object sender, EventArgs e)
        {
            string[] portlar = SerialPort.GetPortNames(); // portlarımızı çektik
            PortComboBox.Items.Clear();
            foreach (string port in portlar) //portların içerisindeki her bir veriyi port'un içini atıyoruz
            {
                PortComboBox.Items.Add(port);
            }



        }



        private void DisconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();
                    //timer1.Stop();
                    RefreshButton.Enabled = true;
                    PortComboBox.Enabled = true;
                    bunifuShapes1.FillColor = Color.Red;

                    //serialPort1.DiscardInBuffer();  // seri portunuzun bufferını temizlemek için kullanılan geri dönüş değeri ve parametresi olmayan fonksiyondur.

                    //BaglantiDurumu = true;


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "hata");

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

                serialPort1.Close();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "hata");

            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {

                data = serialPort1.ReadLine();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "hata");

            }


            this.BeginInvoke(new EventHandler(ProcessData));
        }

        private void ProcessData(object sender, EventArgs e)
        {

            try
            {


                //data = serialPort1.ReadLine();
                splitted_data = data.Split('|');

                

                listBox1.Items.Add(data);
                listBox1.TopIndex = listBox1.Items.Count - 1;

                



                EnlemRTextBox.Text = splitted_data[0];
                BoylamRTextBox.Text = splitted_data[1];
                SicaklikRTextBox.Text = splitted_data[2];
                XIvmeRTextBox.Text = splitted_data[3];
                YIvmeRTextBox.Text = splitted_data[4];
                ZIvmeRTextBox.Text = splitted_data[5];
                XGyroRTextBox.Text = splitted_data[6];
                YGyroRTextBox.Text = splitted_data[7];
                ZGyroRTextBox.Text = splitted_data[8];
                XAngleRTextBox.Text = splitted_data[9];
                YAngleRTextBox.Text = splitted_data[9];

                x = Convert.ToInt64(XAngleRTextBox.Text);
                y = Convert.ToInt64(YAngleRTextBox.Text);
                glControl1.Invalidate();

                kaydet();  // HER DÖNGÜDE TEXTBOX'A VERİYİ YAZDIRIYOR

                string yeniSicaklikDeger = SicaklikRTextBox.Text.Replace('.', ',');
                float degersicaklik = float.Parse(yeniSicaklikDeger);          //SICAKLIK GRAGİĞİ ANCAK 25,14 GİBİ VİRGÜL ŞEKİLDE
                //double sicaklik = Convert.ToDouble(SicaklikRTextBox.Text);   //SICAKLIK GRAGİĞİ ANCAK 25.14 GİBİ NOKTALI ŞEKİLDE
                DateTime myDateValue = DateTime.Now;
                //this.chart1.Series[0].Points.AddXY(myDateValue.ToString("ss"), sicaklik);   // "d/M/yyyy HH:mm:ss" tarih ve saat
                int zaman = 0;
                this.chart1.Series[0].Points.AddXY(zaman, degersicaklik);
                zaman++;

                serialPort1.DiscardInBuffer();
            }
            catch (System.IO.IOException error)
            {
                return;
            }
            catch (System.IndexOutOfRangeException error)
            {
                return;
            }
            catch (System.InvalidOperationException error)
            {
                return;
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            string[] portlar = SerialPort.GetPortNames(); // portlarımızı çektik
            PortComboBox.Items.Clear();
            foreach (string port in portlar) //portların içerisindeki her bir veriyi port'un içini atıyoruz
            {
                PortComboBox.Items.Add(port);
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = PortComboBox.Text;
                serialPort1.BaudRate = 9600;
                serialPort1.Open();
                bunifuShapes1.FillColor = Color.Green;
                
                RefreshButton.Enabled = false;
                PortComboBox.Enabled = false;
                //BaglantiDurumu = true;
                //ConnectButton.Text = "Disconnect";

                //timer1.Start();




            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Baud rate not valid.");

            }
            catch (ArgumentException)
            {
                MessageBox.Show("Port name not valid.");

            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied, try close applications that may be using the port.");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "hata");

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (map.Overlays.Count > 0)
            {
                map.Overlays.RemoveAt(0);
                map.Refresh();
            }

            string yeniEnlemDeger = EnlemRTextBox.Text.Replace('.', ',');
            string yeniBoylamDeger = BoylamRTextBox.Text.Replace('.', ',');
            float degerEnlem = float.Parse(yeniEnlemDeger);
            float degerBoylam = float.Parse(yeniBoylamDeger);
            

            siticoneTextBox14.Text = Convert.ToString(degerEnlem);
            siticoneTextBox13.Text = Convert.ToString(degerBoylam);

            map.DragButton = MouseButtons.Left;

            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            //double enl = Convert.ToDouble(EnlemRTextBox.Text);
            //double boy = Convert.ToDouble(BoylamRTextBox.Text);

            map.DragButton = MouseButtons.Left;
            map.CanDragMap = true;
            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            map.Position = new PointLatLng(degerEnlem, degerBoylam);
            map.MinZoom = 0;
            map.MaxZoom = 23;
            map.Zoom = 5;



            PointLatLng point = new PointLatLng(degerEnlem, degerBoylam);
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
            GMapOverlay markers = new GMapOverlay("markers");
            map.Overlays.Add(markers);
            markers.Markers.Add(marker);


        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            double enlem = 0;
            double boylam = 0;



            map.DragButton = MouseButtons.Left;

            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;

            double enl = Convert.ToDouble(EnlemRTextBox.Text);
            double boy = Convert.ToDouble(BoylamRTextBox.Text);

            map.DragButton = MouseButtons.Left;
            map.CanDragMap = true;
            map.MapProvider = GMapProviders.GoogleSatelliteMap;
            map.Position = new PointLatLng(enl, boy);
            map.MinZoom = 0;
            map.MaxZoom = 23;
            map.Zoom = 5;



            PointLatLng point = new PointLatLng(enl, boy);
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red_dot);
            GMapOverlay markers = new GMapOverlay("markers");
            map.Overlays.Add(markers);
            markers.Markers.Add(marker);
            

        }




        private void bunifuShapes1_ShapeChanged(object sender, Bunifu.UI.WinForms.BunifuShapes.ShapeChangedEventArgs e)
        {

        }

        void kaydet()
        {
            FileStream fsadi = new FileStream("c:/Users/firmi/Desktop/veri.txt", FileMode.Create, FileAccess.Write);
            StreamWriter swadi = new StreamWriter(fsadi);
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                swadi.WriteLine(listBox1.Items[i].ToString());
            }
            fsadi.Close();


        }

        private void KaydetButton_Click(object sender, EventArgs e)


        {
            kaydet();

        }



        private void glControl1_Paint_1(object sender, PaintEventArgs e)
        {
            float step = 1.0f;
            float topla = step;
            float radius = 1.0f;   // ROKETİN GENİŞLİĞİ
            float dikey1 = radius, dikey2 = -radius;
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(1.04f, 4 / 3, 1, 10000);
            Matrix4 lookat = Matrix4.LookAt(18, 0, 0, 0, 0, 0, 0, -1, 0);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.LoadMatrix(ref perspective);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.LoadMatrix(ref lookat);
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            GL.Rotate(x, 1.0, 0.0, 0.0); // ARDUNİO'DAN ALDIĞIMIZ X VERİSİ BURADA İŞLENDİR
            GL.Rotate(y, 0.0, 1.0, 0.0);  // Y=0, ARDUİNO'DAN Y EKSENİNİ ALDIĞIMIZDA ROKET SAPMA YAŞIYOR... GEREK YOK
            GL.Rotate(0, 0.0, 0.0, 1.0);   // z=0

            ortaSilindir(step, topla, radius, 8, -5); // 3 YERİNE 10 YAP SİLİNDİRİN UZUNLUĞU
            koni(0.01f, 0.01f, radius, 0.1f, 8, 8.7f);  // 2.0f ARKA TARAFTAKİ ÇIKINTININ ENİ
            koni(0.01f, 0.01f, radius, 0.0f, -5.0f, -9.0f);
            

            GL.Begin(PrimitiveType.Polygon);  // KANATÇIK1
            GL.Color3(Color.FromArgb(250, 250, 200));
            GL.Vertex3(0.0, 7.9, -3.0);
            GL.Vertex3(0.0, 7.9, 0.0);
            GL.Vertex3(0.0, 4, 0.0);
            GL.Vertex3(0.0, 6.5, -3.0);
            GL.End();

            GL.Begin(PrimitiveType.Polygon); // KANATÇIK2
            GL.Color3(Color.FromArgb(250, 250, 200));
            GL.Vertex3(0.0, 7.9, 3.0);
            GL.Vertex3(0.0, 7.9, 0.0);
            GL.Vertex3(0.0, 4, 0.0);
            GL.Vertex3(0.0, 6.5, 3.0);
            GL.End();

            GL.Begin(PrimitiveType.Lines); // ARKADAKİ BEYAZ ÇIKINTI
            GL.Color3(Color.FromArgb(250, 250, 200));
            GL.Vertex3(0.0, -5.0, 0.0);
            GL.Vertex3(0.0, 9.4, 0.0);
            GL.End();

            
            glControl1.SwapBuffers();
        }
        private void glControl1_Load_1(object sender, EventArgs e)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Enable(EnableCap.DepthTest);
        }
        private void ortaSilindir(float step, float topla, float radius, float dikey1, float dikey2)
        {
            float eski_step = 0.1f;
            GL.Begin(PrimitiveType.Quads);
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 90)
                    GL.Color3(Color.FromArgb(185, 32, 34)); // ORTADAKİ SİLİNDİRİN RENGİ ( 255, 255, 255 main)
                else if (step < 135)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(185, 32, 34)); //255, 255, 255  main  (bordo = 59, 25, 36)


                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 2) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 2) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
            GL.Begin(PrimitiveType.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// Ust Kapak
            {
                if (step < 45) // BU KOD = KONİ VE SİLİNDİRİN BİRLEŞİMİNDEKİ DAİRESEL ÇİZGİDİR
                    GL.Color3(Color.FromArgb(185, 32, 34));  //255, 1, 1    0,0,0 main    
                else if (step < 90)
                    GL.Color3(Color.FromArgb(185, 32, 34)); //250,250,200 main
                else if (step < 135)
                    GL.Color3(Color.FromArgb(185, 32, 34)); //0,0,0
                else if (step < 180)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(185, 32, 34));  // mavi 73,137,243
                else if (step < 270)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(185, 32, 34));


                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);

                GL.Vertex3(ciz1_x, dikey1, ciz1_y);
                GL.Vertex3(ciz2_x, dikey1, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            while (step <= 180) //Alt Kapak
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(185, 32, 34));  //255, 1, 1
                else if (step < 90)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(185, 32, 34));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(185, 32, 34)); //250, 250, 200

                float ciz1_x = (float)(radius * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();
        }
        private void koni(float step, float topla, float radius1, float radius2, float dikey1, float dikey2)
        {
            float eski_step = 0.01f;
            GL.Begin(PrimitiveType.Lines);  
            while (step <= 360)
            {
                if (step < 45)
                    GL.Color3(0.73, 0.13, 0.13);  // ALT VE ÜST KONİDEKİ BEYAZ YERLER  1.0, 1.0, 1.0
                else if (step < 90)
                    GL.Color3(0.73, 0.13, 0.13); // ALT VE ÜST KONİDEKİ BORDO YERLER   bordo = 0.23, 0.10, 0.14
                else if (step < 135)
                    GL.Color3(0.73, 0.13, 0.13);
                else if (step < 180)
                    GL.Color3(0.73, 0.13, 0.13);
                else if (step < 225)
                    GL.Color3(0.73, 0.13, 0.13);
                else if (step < 270)
                    GL.Color3(0.73, 0.13, 0.13);
                else if (step < 315)
                    GL.Color3(0.73, 0.13, 0.13);
                else if (step < 360)
                    GL.Color3(0.73, 0.13, 0.13);


                float ciz1_x = (float)(radius1 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius1 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey1, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            GL.End();

            GL.Begin(PrimitiveType.Lines);
            step = eski_step;
            topla = step;
            while (step <= 180)// UST KAPAK
            {
                if (step < 45)
                    GL.Color3(Color.FromArgb(0, 0, 0)); //255, 1, 1
                else if (step < 90)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 135)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 180)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 225)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 270)
                    GL.Color3(Color.FromArgb(250, 250, 200));
                else if (step < 315)
                    GL.Color3(Color.FromArgb(0, 0, 0));
                else if (step < 360)
                    GL.Color3(Color.FromArgb(250, 250, 200));


                float ciz1_x = (float)(radius2 * Math.Cos(step * Math.PI / 180F));
                float ciz1_y = (float)(radius2 * Math.Sin(step * Math.PI / 180F));
                GL.Vertex3(ciz1_x, dikey2, ciz1_y);

                float ciz2_x = (float)(radius2 * Math.Cos((step + 180) * Math.PI / 180F));
                float ciz2_y = (float)(radius2 * Math.Sin((step + 180) * Math.PI / 180F));
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);

                GL.Vertex3(ciz1_x, dikey2, ciz1_y);
                GL.Vertex3(ciz2_x, dikey2, ciz2_y);
                step += topla;
            }
            step = eski_step;
            topla = step;
            GL.End();
        }

    }
}
