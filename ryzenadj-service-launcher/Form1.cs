using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess; //for checking service
using MetroFramework.Forms; //obvious to what this is
using System.IO; //delete stuff
namespace ryzenadj_service_launcher {
    public partial class Form1 : MetroForm {
        public Form1() {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e) {
            //cleanup
            File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
            File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
            File.Delete("ryzenadj-service.InstallState"); //Log for installing service
            //let's check to see if service started or not :)
            if (serviceexists("ryzenadj-service")) {
                metroButton2.Text = "uninstall";
                metroButton2.Enabled = false;
                //check if running
                if (serviceisrunning("ryzenadj-service")) {
                    metroButton1.Text = "stop service";
                    metroButton2.Enabled = false;
                }
                else {
                    metroButton1.Text = "start service";
                    metroButton1.Enabled = true;
                }
            }
            else {
                metroButton1.Text = "start service";
                metroButton2.Text = "install";
                metroButton1.Enabled = false;
                metroButton2.Enabled = true;
            }
        }
        public bool serviceexists(string name) {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(name));
        }
        public bool serviceisrunning(string name) {
            ServiceController sc = new ServiceController {
                ServiceName = name
            };
            if (sc.Status == ServiceControllerStatus.Running) {
                return true;
            }
            else {
                return false;
            }
        }
        public void stopservice(string name) {
            ServiceController sc = new ServiceController {
                ServiceName = name
            };
            if (sc.Status == ServiceControllerStatus.Running) {
                try {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                catch {
                    MessageBox.Show("couldnt stop the service", "error");
                }
            }
        }
        public void startservice(string name) {
            ServiceController sc = new ServiceController {
                ServiceName = name
            };
            if (sc.Status == ServiceControllerStatus.Stopped) {
                try {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running);
                }
                catch {
                    MessageBox.Show("couldnt start the service", "error");
                }
            }
        }
        private void metroButton1_Click(object sender, EventArgs e) {
            //kaboom
            metroButton2.Enabled = false;
            if (serviceexists("ryzenadj-service")) {
                if (metroButton1.Text == "start service") {
                    if (serviceisrunning("ryzenadj-service")) {
                        File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                        File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                        File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                        metroButton1.Text = "stop service";
                        metroButton1.Enabled = true;
                        metroButton2.Text = "uninstall";
                        metroButton2.Enabled = false;
                        MessageBox.Show("service already started!", "error");
                    }
                    else {
                        startservice("ryzenadj-service");
                        File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                        File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                        File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                        metroButton2.Enabled = false;
                        metroButton1.Text = "stop service";
                        metroButton1.Enabled = true;
                        metroButton2.Text = "uninstall";
                        MessageBox.Show("started the service!", "success");
                    }
                }
                else {
                    if (serviceisrunning("ryzenadj-service")) {
                        stopservice("ryzenadj-service");
                        File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                        File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                        File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                        metroButton2.Enabled = true;
                        metroButton1.Text = "start service";
                        metroButton1.Enabled = true;
                        metroButton2.Text = "uninstall";
                        MessageBox.Show("stopped the service!", "success");
                    }
                    else {
                        File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                        File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                        File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                        metroButton1.Text = "stop service";
                        metroButton1.Enabled = true;
                        metroButton2.Text = "uninstall";
                        metroButton2.Enabled = true;
                        MessageBox.Show("service isnt running!", "error");
                    }
                }
            }
            else {
                metroButton1.Text = "start service";
                metroButton1.Enabled = false;
                metroButton2.Text = "install";
                metroButton2.Enabled = true;
                MessageBox.Show("you need to install the service first!", "error");
            }
            metroButton2.Enabled = true;
        }
        private void metroButton2_Click(object sender, EventArgs e) {
            //let's check to see if installed and if not, then let's install it :)
            if (metroButton2.Text == "install") {
                if (serviceexists("ryzenadj-service")) {
                    if (serviceisrunning("ryzenadj-service")) {
                        metroButton1.Text = "stop service";
                        metroButton2.Enabled = false;
                    }
                    metroButton1.Enabled = true;
                    metroButton2.Text = "uninstall";
                    File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                    File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                    File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                    MessageBox.Show("already installed!", "error");
                }
                else {
                    string runtimedir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo {
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                        FileName = "cmd.exe",
                        Arguments = "/C " + runtimedir + "//installutil.exe ryzenadj-service.exe" // /C is to run and terminate :)
                    };
                    process.StartInfo = startInfo;
                    process.Start();
                    File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                    File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                    File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                    metroButton1.Enabled = true;
                    metroButton2.Enabled = false;
                    metroButton2.Text = "uninstall";
                    MessageBox.Show("service installed!", "success");
                }
            }
            else {
                if (serviceexists("ryzenadj-service")) {
                    if (serviceisrunning("ryzenadj-service")) {
                        metroButton2.Enabled = false;
                        metroButton2.Text = "uninstall";
                        metroButton1.Text = "stop service";
                        metroButton1.Enabled = true;
                        MessageBox.Show("you need to stop the service first!", "error");
                    }
                    else {
                        string runtimedir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo {
                            WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                            FileName = "cmd.exe",
                            Arguments = "/C " + runtimedir + "//installutil.exe /u ryzenadj-service.exe" // /C is to run and terminate :)
                        };
                        process.StartInfo = startInfo;
                        process.Start();
                        File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                        File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                        File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                        metroButton2.Text = "install";
                        metroButton1.Text = "start service";
                        metroButton1.Enabled = false;
                        metroButton2.Enabled = true;
                        MessageBox.Show("service uninstalled!", "success");
                    }
                }
                else {
                    metroButton2.Text = "install";
                    metroButton1.Text = "start service";
                    metroButton2.Enabled = true;
                    metroButton1.Enabled = false;
                    File.Delete("InstallUtil.InstallLog"); //Log for installing/uninstalling service
                    File.Delete("ryzenadj-service.InstallLog"); //Log for installing service
                    File.Delete("ryzenadj-service.InstallState"); //Log for installing service
                    MessageBox.Show("already uninstalled!", "error");
                }
            }
        }
    }
}
