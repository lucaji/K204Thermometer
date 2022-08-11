using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using RJCP.IO.Ports;

using ScottPlot;
using ScottPlot.Plottable;

#nullable enable

namespace k204 {

    public partial class K204Form : Form {

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public K204Form() {
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

            InitializeComponent();

            PlotInit();

            this.hasAutoscaleMenuItem = new ToolStripMenuItem("Autoscale", null, new EventHandler(RightClickMenu_AutoAxis_Click));
            this.hasGradientFillMenuItem = new ToolStripMenuItem("Gradient fill", null, new EventHandler(RightClickMenu_GradientFill_Click));

            FormClosed += (sender, args) => {
                theSerialPort?.Close();
            };

            SettingsGetComPort();
        }


        int nextDataIndex = 1;
        const int DataLimit = 360_000; // 100 hours at 1 Hz rate
        public double[] Ch1Data = new double[DataLimit];
        public double[] Ch2Data = new double[DataLimit];
        public double[] Ch3Data = new double[DataLimit];
        public double[] Ch4Data = new double[DataLimit];
        SignalPlot Ch1Plot;
        SignalPlot Ch2Plot;
        SignalPlot Ch3Plot;
        SignalPlot Ch4Plot;


        void PlotInit() {
            // Replace context click menu event
            formsPlot1.RightClicked -= formsPlot1.DefaultRightClickEvent;
            formsPlot1.RightClicked += CustomRightClickEvent;

            formsPlot1.Plot.Style(Style.Gray1);

            Ch1Plot = formsPlot1.Plot.AddSignal(Ch1Data);
            Ch1Plot.YAxisIndex = 0;

            Ch2Plot = formsPlot1.Plot.AddSignal(Ch2Data);
            //Ch2Plot.YAxisIndex = 1;

            Ch3Plot = formsPlot1.Plot.AddSignal(Ch3Data);
            //Ch3Plot.YAxisIndex = 2;

            Ch4Plot = formsPlot1.Plot.AddSignal(Ch4Data);
            //Ch4Plot.YAxisIndex = 3;


            formsPlot1.Plot.XLabel("Time");
            //formsPlot1.Plot.XAxis.DateTimeFormat(true);

            formsPlot1.Plot.YLabel("°C");
            formsPlot1.Plot.YAxis.Color(Ch1Plot.Color);

            formsPlot1.AutoSize = true;

            formsPlot1.Refresh();
        }

        void AddDiagramPoint(double ch1, double ch2, double ch3, double ch4) {
            if (nextDataIndex >= DataLimit) {
                nextDataIndex = 1;
            }

            Ch1Data[nextDataIndex] = ch1;
            Ch2Data[nextDataIndex] = ch2;
            Ch3Data[nextDataIndex] = ch3 == 32767 ? 0 : ch3;
            Ch4Data[nextDataIndex] = ch4 == 32767 ? 0 : ch3;

            Ch1Plot.MaxRenderIndex =
            Ch2Plot.MaxRenderIndex = 
            Ch3Plot.MaxRenderIndex = 
            Ch4Plot.MaxRenderIndex = nextDataIndex++;

            if (HasAutoScale) {
                formsPlot1.Plot.AxisAuto();
            }
            formsPlot1.Render();

        }


        K204 TheDevice = new();

        #region Serial Ports

        List<PortDescription> FoundPorts = new();
        PortDescription PortByName(string name) => FoundPorts.FirstOrDefault(p => p.Port.Equals(name));

        private SerialPortStream? theSerialPort = null;





        private void GetSerials() {
            FoundPorts.Clear();
            foreach (var desc in SerialPortStream.GetPortDescriptions()) {
                System.Diagnostics.Debug.WriteLine("GetPortDescriptions: " + desc.Port + "; Description: " + desc.Description);
                FoundPorts.Add(desc);
            }
        }


        private void CloseSerial(string serialName) {
            timer1.Enabled = false;
            if (string.IsNullOrWhiteSpace(serialName)) {
                MessageBox.Show("Invalid port name to close: " + serialName + ".");
                return;
            }
            if (theSerialPort is null) { return; }
            theSerialPort.Close();
            if (theSerialPort.PortName.Equals(serialName)) {
                WFchangedPortStatus();
            }
            SerialPortConnectButton.Text = "Connect";
        }

        void ConnectToSerial(string serialName) {
            if (string.IsNullOrWhiteSpace(serialName)) { return; }
            if (theSerialPort != null && theSerialPort.PortName == serialName) {
                CloseSerial(serialName); 
                return;
            }
            var p = PortByName(serialName);
            if (p == default) { return; }
            Console.WriteLine("Connecting to " + serialName);
            theSerialPort = new SerialPortStream(serialName, 9600, 8, Parity.None, StopBits.One);
            try {
                theSerialPort.Open();
                WFchangedPortStatus();
                SerialPortConnectButton.Text = "Disconnect";
                timer1.Enabled = true;
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion


        


        void ReadK204Packet() {
            if (!(theSerialPort?.IsOpen ?? false)) { return; }
            try {
                if (theSerialPort.CanWrite) {
                    theSerialPort.WriteByte(0x41); // send 'A' command to the K204
                } else {
                    System.Diagnostics.Debug.WriteLine("cannot write");
                    return;
                }
                if (theSerialPort.CanRead) {
                    var bc = theSerialPort.BytesToRead;
                    if (bc != 45) {
                        System.Diagnostics.Debug.WriteLine("got only " + bc + " bytes available from serial...sheesh");
                    }
                    byte[] buffero = new byte[bc];
                    int offset = 0;
                    var success = theSerialPort.Read(buffero, offset, bc);
                    if (success == -1) {
                        System.Diagnostics.Debug.WriteLine("cant read -1");
                        return;
                    }
                    if (TheDevice.ParseK204DataBuffer(ref buffero)) {
                        AddDiagramPoint(
                            TheDevice.Channel1Data.Last().TemperatureValue,
                            TheDevice.Channel2Data.Last().TemperatureValue,
                            TheDevice.Channel3Data.Last().TemperatureValue,
                            TheDevice.Channel4Data.Last().TemperatureValue
                            );
                    }
                } else {
                    System.Diagnostics.Debug.WriteLine("less than 45 bytes received");
                }
            } catch (Exception ex) {
                CloseSerial(theSerialPort.PortName);
                MessageBox.Show(ex.Message);
            }
        }


        void WFchangedPortStatus() {
            if (theSerialPort == null || theSerialPort.IsDisposed) {
                this.SerialPortConnectButton.Text = "Disposed";
                this.SerialPortConnectButton.Enabled = false;
                return;
            } else if (theSerialPort.IsOpen) {
                this.SerialPortConnectButton.Text = "Disconnect";
            } else {
                this.SerialPortConnectButton.Text = "Connect";
            }
            this.SerialPortConnectButton.Enabled = true;
        }



        void DiagramReset() {
            nextDataIndex = 1;
            Ch1Plot.MaxRenderIndex = 
            Ch2Plot.MaxRenderIndex = 
            Ch3Plot.MaxRenderIndex = 
            Ch4Plot.MaxRenderIndex = nextDataIndex;
            if (HasAutoScale) {
                formsPlot1.Plot.AxisAuto();
            }
            formsPlot1.Render();
        }

        void ExportDataCsv(string pathname) {
            var myExport = new CsvExport();
            for (int i = 0; i < nextDataIndex; i++) {
                myExport.AddRow();
                myExport["Timestamp"] = TheDevice.Channel1Data[i].TimeStamp.ToString("F");
                myExport["Ch1"] = TheDevice.Channel1Data[i].TemperatureValue.ToString();
                myExport["Ch2"] = TheDevice.Channel2Data[i].TemperatureValue.ToString();
                myExport["Ch3"] = TheDevice.Channel3Data[i].TemperatureValue.ToString();
                myExport["Ch4"] = TheDevice.Channel4Data[i].TemperatureValue.ToString();
            }
            myExport.ExportToFile(pathname);
            // FileShellUtils.ExecuteAtPath(pathname);
        }


        void DisplayExportDataDialog() {
            var filename = "temp"; //  StringUtils.DateNowFileFormat() + " GPM8310";
            var sfd = new SaveFileDialog {
                FileName = filename + ".csv",
                Filter = "CSV Files (*.csv)|*.csv;*.csv" +
                         "|TXT Files (*.txt)|*.txt;*.txt" +
                         "|All files (*.*)|*.*"
            };

            if (sfd.ShowDialog() == DialogResult.OK) {
                ExportDataCsv(sfd.FileName);
            }
        }

        #region Plot Custom Context Menu

        readonly ToolStripMenuItem hasAutoscaleMenuItem;
        readonly ToolStripMenuItem hasGradientFillMenuItem;

        private void CustomRightClickEvent(object sender, EventArgs e) {
            ContextMenuStrip customMenu = new();
            customMenu.Items.Add(hasAutoscaleMenuItem);
            customMenu.Items.Add(hasGradientFillMenuItem);
            customMenu.Items.Add(new ToolStripMenuItem("Open graph in new window", null, new EventHandler(RightClickMenu_OpenInNewWindow_Click)));
            customMenu.Items.Add(new ToolStripSeparator());
            customMenu.Items.Add(new ToolStripMenuItem("Copy Picture to Clipboard", null, new EventHandler(RightClickMenu_Copy_Click)));
            customMenu.Items.Add(new ToolStripMenuItem("Save Image As...", null, new EventHandler(RightClickMenu_SaveImage_Click)));
            customMenu.Items.Add(new ToolStripMenuItem("Save Data As...", null, new EventHandler(RightClickMenu_SaveData_Click)));
            customMenu.Items.Add(new ToolStripSeparator());
            customMenu.Items.Add(new ToolStripMenuItem("Clear Data", null, new EventHandler(RightClickMenu_ClearData_Click)));
            customMenu.Show(System.Windows.Forms.Cursor.Position);
        }

        private void RightClickMenu_OpenInNewWindow_Click(object sender, EventArgs e) => new FormsPlotViewer(formsPlot1.Plot).Show();

        private void RightClickMenu_ClearData_Click(object sender, EventArgs e) {
            DiagramReset();
        }

        private void RightClickMenu_GradientFill_Click(object sender, EventArgs e) {
            HasGradientFill = !HasGradientFill;
        }

        private void RightClickMenu_SaveImage_Click(object sender, EventArgs e) {
            var filename = "temp"; //  StringUtils.DateNowFileFormat() + " GPM8310";
            var sfd = new SaveFileDialog {
                FileName = filename + ".png",
                Filter = "PNG Files (*.png)|*.png;*.png" +
                         "|JPG Files (*.jpg, *.jpeg)|*.jpg;*.jpeg" +
                         "|BMP Files (*.bmp)|*.bmp;*.bmp" +
                         "|All files (*.*)|*.*"
            };

            if (sfd.ShowDialog() == DialogResult.OK) {
                formsPlot1.Plot.SaveFig(sfd.FileName);
            }
        }

        private void RightClickMenu_SaveData_Click(object sender, EventArgs e) {
            DisplayExportDataDialog();
        }

        private void RightClickMenu_Copy_Click(object sender, EventArgs e) => Clipboard.SetImage(formsPlot1.Plot.Render());

        private void RightClickMenu_AutoAxis_Click(object sender, EventArgs e) {
            HasAutoScale = !HasAutoScale;
        }

        #endregion

        #region plot theming

        private bool _HasGradientFill = false;
        public bool HasGradientFill {
            get { return _HasGradientFill; }
            set {
                _HasGradientFill = value;
                if (value) {
                    Ch1Plot.FillBelow(Color.Blue);
                    Ch1Plot.GradientFillColor1 = Color.Transparent;
                    
                } else {
                    Ch1Plot.FillDisable();

                }


                formsPlot1.Render();
                hasGradientFillMenuItem.Checked = value;
            }
        }

        public bool HasAutoScale {
            get { return hasAutoscaleMenuItem.Checked; }
            set {
                hasAutoscaleMenuItem.Checked = value;
                if (value) {
                    formsPlot1.Plot.AxisAuto();
                }
            }
        }


        #endregion

        private void Timer1_Tick(object sender, EventArgs e) {
            ReadK204Packet();
        }

        private void SerialPortConnectButton_Click(object sender, EventArgs e) {
            if (this.SerialPortDropDown.DropDownItems.Count == 0) { return; }
            var serialName = this.SerialPortDropDown.Text;
            ConnectToSerial(serialName);
        }

        private void SerialPortDropDown_DropDownOpening(object sender, EventArgs e) {
            this.SerialPortDropDown.DropDownItems.Clear();
            GetSerials();
            foreach (var port in FoundPorts) {
                this.SerialPortDropDown.DropDownItems.Add(port.Port);
            }
        }

        private void SerialPortDropDown_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            var portname = e.ClickedItem.Text;
            if (portname.StartsWith("COM")) {
                SettingsSaveComPort(portname);
            }
        }

        void SettingsSaveComPort(string portname) {
            if (string.IsNullOrWhiteSpace(portname)) { return; }
            Properties.Settings.Default.LastCOMPort = portname;
            Properties.Settings.Default.Save();
            SettingsGetComPort();
        }

        void SettingsGetComPort() {
            var portname = Properties.Settings.Default.LastCOMPort;
            if (string.IsNullOrWhiteSpace(portname)) { return; }
            SerialPortDropDown.Text = portname;
        }
    }
}
