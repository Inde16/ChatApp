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
using System.Media;

namespace Chat_App
{
    public partial class frmMain : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;

        public frmMain()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            Random rnd = new Random();
            txtUser.Text = "User" + rnd.Next(1, 1000);
            txtLocalIp.Text = GetLocalIP();
            txtOtherIp.Text = GetLocalIP();

        }

        private string GetLocalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        private void btn_str_Click(object sender, EventArgs e)
        {
            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(txtLocalIp.Text), Convert.ToInt32(txtLocalPort.Text));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(txtOtherIp.Text), Convert.ToInt32(txtOtherPort.Text));

                sck.Connect(epRemote);


                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                btn_str.Text = "Connected";
                btn_str.Enabled = false;
                btn_send.Enabled = true;
                txtMessage.Focus();
              

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }



        private void btn_send_Click(object sender, EventArgs e)
        {
            try
            {
                //Convert message from sting to byte
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(txtMessage.Text);

                sck.Send(msg);
                lstMessage.Items.Add(txtUser.Text+ ": " + txtMessage.Text);
                txtMessage.Clear();
                

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        
        


        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Pissaris Sotiris" + Environment.NewLine + "Softaware Engineer" + Environment.NewLine + "Iek Delta Athens");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1464];

                    receivedData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);
                    lstMessage.Items.Add(txtUser.Text + ": " + receivedMessage);
                    Console.Beep();
                }

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length , SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer); 
                

                
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }


    }
}
