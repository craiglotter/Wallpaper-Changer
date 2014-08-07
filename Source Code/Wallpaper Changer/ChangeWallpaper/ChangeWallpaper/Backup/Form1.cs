using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO; 
using System.Data;
using Microsoft.Win32 ;

namespace ChangeWallpaper
{
	using System.Net ;

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.ComboBox comboBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(8, 8);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(312, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "C:\\Documents and Settings\\All Users\\Documents\\My Pictures\\Sample Pictures\\winter." +
				"jpg";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(336, 8);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.TabIndex = 1;
			this.btnBrowse.Text = "&Browse";
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnApply
			// 
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.Location = new System.Drawing.Point(360, 64);
			this.btnApply.Name = "btnApply";
			this.btnApply.TabIndex = 1;
			this.btnApply.Text = "&Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.Items.AddRange(new object[] {
																									 "Stretched",
																									 "Centered",
																									 "Tiled"});
			this.comboBox1.Location = new System.Drawing.Point(8, 40);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(312, 21);
			this.comboBox1.TabIndex = 2;
			this.comboBox1.Text = "Stretched";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(440, 94);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.btnApply);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void btnBrowse_Click(object sender, System.EventArgs e)
		{
			openFileDialog1.InitialDirectory = @"C:\";
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				textBox1.Text = openFileDialog1.FileName;
			}
		}

		private void btnApply_Click(object sender, System.EventArgs e)
		{
			string s = comboBox1.Text ;
			Wallpaper.Style s2 = (Wallpaper.Style )Enum.Parse( typeof( Wallpaper.Style ), s, false ) ;

			Wallpaper.Set( new Uri( textBox1.Text ), 
				s2 ) ;
		
		}
	}

	public sealed class Wallpaper
	{
		Wallpaper( ) { }

		const int SPI_SETDESKWALLPAPER = 20  ;
		const int SPIF_UPDATEINIFILE = 0x01;
		const int SPIF_SENDWININICHANGE = 0x02;

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		static  extern int SystemParametersInfo (int uAction , int uParam , string lpvParam , int fuWinIni) ;

		public enum Style : int
		{
			Tiled,
			Centered,
			Stretched
		}

		public static void Set ( Uri uri, Style style )
		{
			System.IO.Stream s = new WebClient( ).OpenRead( uri.ToString( ) );

			System.Drawing.Image img = System.Drawing.Image.FromStream( s );
			string tempPath = Path.Combine( Path.GetTempPath( ), "wallpaper.bmp"  ) ;
			img.Save( tempPath ,  System.Drawing.Imaging.ImageFormat.Bmp ) ;

			RegistryKey key = Registry.CurrentUser.OpenSubKey( @"Control Panel\Desktop", true ) ;
			if ( style == Style.Stretched )
			{
				key.SetValue(@"WallpaperStyle", 2.ToString( ) ) ;
				key.SetValue(@"TileWallpaper", 0.ToString( ) ) ;
			}

			if ( style == Style.Centered )
			{
				key.SetValue(@"WallpaperStyle", 1.ToString( ) ) ;
				key.SetValue(@"TileWallpaper", 0.ToString( ) ) ;
			}

			if ( style == Style.Tiled )
			{
				key.SetValue(@"WallpaperStyle", 1.ToString( ) ) ;
				key.SetValue(@"TileWallpaper", 1.ToString( ) ) ;
			}

			SystemParametersInfo( SPI_SETDESKWALLPAPER, 
				0, 
				tempPath,  
				SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE );
		}
	}
}
