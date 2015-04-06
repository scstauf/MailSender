using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MailSender
{
    public partial class frmSendMail : Form
    {
        public frmSendMail()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            bool success = true;
            foreach (var ctrl in this.Controls)
            {
                if (ctrl is TextBox) {
                    if (((TextBox)ctrl).TextLength == 0)
                    {
                        success = false;
                        break;
                    }
                }
            }

            if (!success)
            {
                MessageBox.Show("I need you to fill out everything. Thanks!");
                return;
            }

            int port = 0;
            if (Int32.TryParse(txtPort.Text, out port))
            {
                var addrTo = txtTo.Text;
                var addrFrom = txtFrom.Text;
                var sub = txtSubject.Text;
                var bod = txtMessage.Text;
                var srv = txtHost.Text;
                var usr = txtUsername.Text;
                var pwd = txtPassword.Text;

                var err = new System.Text.StringBuilder();

                try
                {
                    using (var mail = new System.Net.Mail.MailMessage(addrFrom, addrTo, sub, bod))
                    {
                        using (var client = new System.Net.Mail.SmtpClient(srv, port))
                        {
                            client.Credentials = new System.Net.NetworkCredential(usr, pwd);
                            client.Send(mail);
                        }
                    }
                }
                catch (System.Net.Mail.SmtpException x)
                {
                    err.AppendFormat("\r\n\r\nSmtpException!\r\nSmtpStatusCode={0}\r\nError Message={1}\r\nStack Trace={2}", x.StatusCode, x.Message, x.StackTrace);
                }
                catch (Exception x)
                {
                    err.AppendFormat("\r\n\r\nException!\r\nError Message={0}\r\nStack Trace={1}", x.Message, x.StackTrace);
                }

                if (err.ToString().Length > 0)
                {
                    System.IO.File.AppendAllText("MailSender_Error.txt", string.Format("Please send this to Scotty!{0}", err.ToString()));
                    System.Diagnostics.Process.Start("MailSender_Error.txt");
                }
            }
            else
            {
                MessageBox.Show("You didn't enter a number for the port. Please keep it numeric.");
            }
        }
    }
}
