﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleTcp;
namespace Client
{
    public partial class Main_face : Form
    {
        Bitmap image;
        string base64Text;
        SimpleTcpClient client;
        string userNamebetween = "";
        List<ListClietnActi> listClient;
        public Main_face()
        {
            InitializeComponent();
        }

        private void Main_face_Load(object sender, EventArgs e)
        {
            client = new SimpleTcpClient(Sign_In.ip2Form);
            userNamebetween = Sign_In.userName2Form;
            client.Events.Disconnected += Event_Disconnected;
            client.Events.DataReceived += Event_DataReceived;
            try {
                client.Connect();
                client.Send($"4{Sign_In.userName2Form}");
            }
            catch { }
            textNameF.Text = Sign_In.userName2Form;
            //LoadListClientActi();
        }

        private void Event_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // throw new NotImplementedException();
            string s = Encoding.UTF8.GetString(e.Data);
            this.Invoke(new Action(() =>
            {
                
                if (s[0] == '1') {
                    MessageBox.Show("Server is Disconnected", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Invoke(new Action(() => { this.Close(); }));
                }
                else if (s[0] == '6') {
                    guna2TextBox1.Text += $"{s}{Environment.NewLine}";
                    string clietString = "";
                    listClient = new List<ListClietnActi>();
                    flowLayoutPanel1.Controls.Clear();
                    for(int i = 1; i < s.Length; i++) {
                        if (s[i] != '@')
                            clietString += s[i];
                        else if (s[i] == '@') {
                            if (clietString != textNameF.Text) { 
                                ListClietnActi f = new ListClietnActi();
                                f.nameText.Text= clietString;
                                listClient.Add(f);
                                f.Tag = clietString;
                                f.Click += lable_click;
                            }
                            clietString = "";
                            //flowLayoutPanel1.Controls.Add(f);                        
                        }
                    }
                    int j = 0;
                    foreach(ListClietnActi item in listClient) {
                        if (j == 0) {
                            OpText.Text = item.nameText.Text;
                        }j++;
                        flowLayoutPanel1.Controls.Add(item);
                    }
                }
                else if (s[0] == '8') {
                    string text = "";
                    for(int i = 1; i < s.Length; i++) {
                        text += s[i];
                    }
                    Receive r = new Receive();
                    r.guna2TextBox1.Text = text;
                    flowLayoutPanel2.Controls.Add(r);
                }
                else if (s[0] == '9') {
                    string text = "";
                    int count = 0;
                    string nameSend = "", nameRec = "", content = "";
                    guna2TextBox1.Text += $"{Environment.NewLine}";
                    for (int i = 1; i < s.Length; i++) {
                        if (count == 3) {
                            guna2TextBox1.Text += $"{nameSend}\t{nameRec}\t{content}{Environment.NewLine}";
                            nameSend = "";nameRec = "";content = "";
                            count = 0;
                        }
                        i++;string countS = "";
                        while (s[i] != '*') {
                            countS += s[i];
                            i++;
                        }i++;
                        for(int j = 0; j < Int32.Parse(countS); j++) {
                            if (count == 0) nameSend += s[j];
                            else if (count == 1) nameRec += s[j];
                            else if (count == 2) content += s[j];
                            i++;
                        }
                        count++;
                    }
                    //*4*hung*6*hung22*7*sfsdfsf*6*hung22*4*hung*12*sdssfdfsdfsf*4*hung*6*hung22*10*sdsdfsdfsf*6*hung22*4*hung*6*sddfsf*4*hung*6*hung22*8*sdfsdfsf*4*hung*6*hung22*7*dfsdfsf*6*hung22*4*hung*5*sdfsf*4*hung*6*hung22*5*sdfsf
                    //guna2TextBox1.Text += $"{Environment.NewLine}{s.Substring(1)}{Environment.NewLine}";
                }
            }));
        }

        private void Event_Disconnected(object sender, ClientDisconnectedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void circularBtn9_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(messageText.Text)) {
                client.Send($"8{OpText.Text}@{messageText.Text}");
                Send s = new Send();
                s.guna2TextBox1.Text = messageText.Text;
                flowLayoutPanel2.Controls.Add(s);
                messageText.Text = string.Empty;
            }
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            client.Send($"3{userNamebetween}");client.Disconnect();
        }
        // xu ly messenger
        private void LoadListClientActi() {
            client.Send("6"+textNameF.Text);
            /*for(int i = 0; i < 20; i++) {
                ListClietnActi f = new ListClietnActi();
                f.Click += lable_click;
                flowLayoutPanel1.Controls.Add(f);
            }*/

        }

        private void lable_click(object sender, EventArgs e){
            string s = (sender as ListClietnActi).Tag as string;
            OpText.Text = s;
            client.Send($"9{textNameF.Text}@{s}");
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void circularPicture1_Click(object sender, EventArgs e)
        {

        }
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        private void circularBtn11_Click(object sender, EventArgs e)
        {
            Thread t = new Thread((ThreadStart)(() => {
                /*OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG" +
                "|All files(*.*)|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    image = new Bitmap(dialog.FileName);
                    pictureBox1.Image = (Image)image;

                    byte[] imageArray = System.IO.File.ReadAllBytes(dialog.FileName);
                    base64Text = Convert.ToBase64String(imageArray); //base64Text must be global but I'll use  richtext
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.Text = $"0x{ByteArrayToString(imageArray)}";
                    }));
                }*/
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG" +
                "|All files(*.*)|*.*";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    image = new Bitmap(dialog.FileName);
                    pictureBox1.Image = (Image)image;

                    byte[] imageArray = System.IO.File.ReadAllBytes(dialog.FileName);

                    /*var image1 = new ImageConverter().ConvertTo(pictureBox1.Image, typeof(Byte[]));
                    //richTextBox1.Text = image1.GetType().ToString();
                    byte[] arra = conVertImagesToBytes(pictureBox1.Image);*/
                    base64Text = $"0x{ByteArrayToString(imageArray)}";
                }
            }));

            // Run your code from a thread that joins the STA Thread
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

        }
        
        private void BtnUpload_Click(object sender, EventArgs e)
        {
            client.Send(base64Text);
        }
        // xy ly trang chu
    }

}
