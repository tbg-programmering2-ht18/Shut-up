using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace messengerApp
{
    public partial class Form1 : Form
    {
        private TcpClient client; // listen for connection from TCP network
        public StreamWriter STW;
        public StreamReader STR;
        public string recieve;
        public string textToSend;

        public Form1()
        {
            InitializeComponent();
            //The name localhost normally resolves to the IPv4 loopback address 127.0.0.1, and to the IPv6 loopback address ::1
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress address in localIP)
            {
                //Checks for ipv4
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPtextBox.Text = address.ToString();
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            try
            {
                //Check with ex Wireshark 
                TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(ServerPORTtextBox.Text));
                listener.Start();
                this.BackColor = Color.Blue;
                this.Update();
                ChatScreentextBox.AppendText("Server started" + "\n");
                ChatScreentextBox.Update();
                client = listener.AcceptTcpClient(); //Accept a pending connection request 
                STR = new StreamReader(client.GetStream());
                STW = new StreamWriter(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1.RunWorkerAsync();
                backgroundWorker2.WorkerSupportsCancellation = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    recieve = STR.ReadLine();
                    this.ChatScreentextBox.Invoke(new MethodInvoker(delegate ()
                    {
                        ChatScreentextBox.AppendText("Other: " + recieve + "\n");
                    }));
                    recieve = "";

                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());

                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(textToSend);
                this.ChatScreentextBox.Invoke(new MethodInvoker(delegate()
                {
                    ChatScreentextBox.AppendText("Me: "+ textToSend + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            backgroundWorker2.CancelAsync();

        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (MessagetextBox.Text !="")
            {
                textToSend = MessagetextBox.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            MessagetextBox.Text = "";

        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
                try
                {
                    client = new TcpClient();

                    //Represents a network endpoint as an IP address and a port number.
                    IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPtextBox.Text), int.Parse(ClientPORTtextBox.Text));

                    //Connects the client to a remote TCP host using the specified host name and port number.
                    client.Connect(ipEnd);
                    if (client.Connected)
                    {
                        this.BackColor = Color.Green;
                        this.Update();
                        ChatScreentextBox.AppendText("Connected to Server" + "\n");
                        ChatScreentextBox.Update();
                        STR = new StreamReader(client.GetStream());
                        STW = new StreamWriter(client.GetStream());
                        STW.AutoFlush = true;
                        backgroundWorker1.RunWorkerAsync();
                        backgroundWorker2.WorkerSupportsCancellation = true;
                    }
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ChatScreentextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void MessagetextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClientPORTtextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerIPtextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void ClientIPtextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
