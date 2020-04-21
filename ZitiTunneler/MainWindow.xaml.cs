﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Windows.Media.Animation;
using ZitiTunneler.Models;
using System.Windows.Forms;

namespace ZitiTunneler {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow:Window {

		public NotifyIcon notifyIcon;

		private List<ZitiIdentity> identities = new List<ZitiIdentity>();
		private List<ZitiService> services = new List<ZitiService>();
		public MainWindow() {
			InitializeComponent();
			App.Current.MainWindow.WindowState = WindowState.Normal;

			notifyIcon = new NotifyIcon();
			notifyIcon.Visible = true;
			notifyIcon.Click += TargetNotifyIcon_Click;

			SetNotifyIcon("white");

			InitializeComponent();
		}

		private void TargetNotifyIcon_Click(object sender, EventArgs e) {
			if (App.Current.MainWindow.WindowState==WindowState.Minimized) {
				App.Current.MainWindow.WindowState = WindowState.Normal;
				App.Current.MainWindow.BringIntoView();
				//this.Opacity = 1;
			} else {
				App.Current.MainWindow.WindowState = WindowState.Minimized;
				//this.Opacity = 0;
			}
		}
		
		private void MainWindow1_Loaded(object sender, RoutedEventArgs e) {
			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
			this.Left = desktopWorkingArea.Right-this.Width-25;
			this.Top = desktopWorkingArea.Bottom-this.Height-25;
			CreateFakeData();
			LoadIdentities();
		}

		private void CreateFakeData() {
			services.Add(new ZitiService("Hush Services","https://hughservice:80"));
			services.Add(new ZitiService("mPOS Service", "https://mps:8080"));
			identities.Add(new ZitiIdentity("Jeremy-PC", "demo.ziti.controller.com:1280", true, services.ToArray()));
			services.Add(new ZitiService("eugenes secure hard drive", "C:\\delete\\*.*"));
			identities.Add(new ZitiIdentity("Jeremy-iPaq", "ziti.netfoundry.io:1408", false, services.ToArray()));
			services.Add(new ZitiService("Red Tube Access", "https://tubered.com:22"));
			services.Add(new ZitiService("Storage Services", "https://aureafit:21"));
			identities.Add(new ZitiIdentity("Hart-Mac", "ziti.supersecret.io:1408", true, services.ToArray()));
		} 
		private void SetNotifyIcon(string iconPrefix) {
			System.IO.Stream iconStream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,/Assets/Images/ziti-"+iconPrefix+".ico")).Stream;
			notifyIcon.Icon = new System.Drawing.Icon(iconStream);
		}

		private void LoadIdentities() {
			IdList.Children.Clear();
			ZitiIdentity[] ids = identities.ToArray();
			for (int i=0; i<ids.Length; i++) {
				IdentityItem id = new IdentityItem();
				id.Identity = ids[i];
				id.OnClick += OpenIdentity;
				IdList.Children.Add(id);
			}
			UIMain.Height = 480+(identities.Count*60);
			BgColor.Height = 480+(identities.Count*60);
			App.Current.MainWindow.Height = 490+(identities.Count*60);
			var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
			this.Left = desktopWorkingArea.Right-this.Width-25;
			this.Top = desktopWorkingArea.Bottom-this.Height-25;
		}

		private void OpenIdentity(ZitiIdentity identity) {
			IdentityMenu.Identity = identity;
			IdentityMenu.Visibility = Visibility.Visible;
		}

		private void ShowMenu(object sender, MouseButtonEventArgs e) {
			MainMenu.Visibility = Visibility.Visible;
		}

		private void AddIdentity(object sender, MouseButtonEventArgs e) {
			Microsoft.Win32.OpenFileDialog jwtDialog = new Microsoft.Win32.OpenFileDialog();
			jwtDialog.DefaultExt = ".jwt";
			jwtDialog.Filter = "Ziti Identities (*.jwt)|*.jwt";
			if (jwtDialog.ShowDialog() == true) {
				string fileContent = File.ReadAllText(jwtDialog.FileName);
				// Clint!! AxedaBuddy - What to do with the jwt file?
			}
		}

		private void Connect(object sender, RoutedEventArgs e) {
			SetNotifyIcon("green");
			ConnectButton.Visibility = Visibility.Collapsed;
			DisconnectButton.Visibility = Visibility.Visible;
		}
		private void Disconnect(object sender, RoutedEventArgs e) {
			SetNotifyIcon("white");
			ConnectButton.Visibility = Visibility.Visible;
			DisconnectButton.Visibility = Visibility.Collapsed;
		}
	}
}