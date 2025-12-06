using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FireRobot
{
    public partial class Form1 : Form
    {
        private ColorCircle statusCircle;

        private TcpListener _listener;
        private TcpClient _client;
        private CancellationTokenSource _cts;

        public Form1()
        {
            InitializeComponent();

            statusCircle = new ColorCircle
            {
                Location = new Point(470, 0),
                CircleColor = Color.Green  // initial: NORMAL
            };

            this.Controls.Add(statusCircle);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async void ConnectBtn_Click(object sender, EventArgs e)
        {
            string ipText = IPBox.Text.Trim();
            string portText = PortBox.Text.Trim();

            int port = int.Parse(portText);

            IPAddress ip;
            if (string.IsNullOrWhiteSpace(ipText) || ipText == "0.0.0.0")
                ip = IPAddress.Any;
            else
                ip = IPAddress.Parse(ipText);

            _listener = new TcpListener(ip, port);
            _listener.Start();

            ConnectBtn.Enabled = false;
            ConnectBtn.Text = "Waiting...";

            _cts = new CancellationTokenSource();

            _client = await _listener.AcceptTcpClientAsync();

            ConnectBtn.Text = "Connected";

            _ = Task.Run(() => ReceiveLoop(_client, _cts.Token));
        }

        private async Task ReceiveLoop(TcpClient client, CancellationToken token)
        {
            var buffer = new byte[1024];
            var sb = new StringBuilder();
            var stream = client.GetStream();

            while (!token.IsCancellationRequested)
            {
                int n;
                try
                {
                    n = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                }
                catch
                {
                    break;
                }

                if (n <= 0)
                    break;

                sb.Append(Encoding.ASCII.GetString(buffer, 0, n));

                while (true)
                {
                    string current = sb.ToString();
                    int idx = current.IndexOf('\n');
                    if (idx < 0)
                        break;

                    string line = current.Substring(0, idx).Trim();
                    sb.Remove(0, idx + 1);

                    HandleMessage(line);
                }
            }

            client.Close();
            _listener.Stop();
            _listener = null;

            if (!IsDisposed && !Disposing)
            {
                BeginInvoke(new Action(() =>
                {
                    ConnectBtn.Enabled = true;
                    ConnectBtn.Text = "Connect";
                }));
            }
        }

        private void HandleMessage(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(HandleMessage), msg);
                return;
            }

            // 1. update TextBox3
           // textBox3.Text = msg;

            // 2. update circle color
            if (msg == "DETECTED")
            {
                statusCircle.CircleColor = Color.Red;
            }
            else
            {
                statusCircle.CircleColor = Color.Green;
            }

            // 3. force redraw
            statusCircle.Invalidate();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cts?.Cancel();
            _client?.Close();
            _listener?.Stop();
            base.OnFormClosing(e);
        }
    }
}
